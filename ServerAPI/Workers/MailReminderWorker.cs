using System.Net;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;
using Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerAPI.Repositories;
using ServerAPI.Repositories.Calendars;

namespace ServerAPI.Workers;

public class MailReminderWorker : BackgroundService
{
    private readonly ILogger<MailReminderWorker> _log;
    private readonly ICalendarRepository _calRepo;
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;

    public MailReminderWorker(
        ILogger<MailReminderWorker> log,
        ICalendarRepository calRepo,
        IUserRepository userRepo,
        IConfiguration config)
    {
        _log = log;
        _calRepo = calRepo;
        _userRepo = userRepo;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Kør straks ved opstart:
        await RunOnce(stoppingToken);

        // Kør derefter hver time (justér evt. til 30 min.)
        var interval = TimeSpan.FromHours(1);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, stoppingToken);
                await RunOnce(stoppingToken);
            }
            catch (TaskCanceledException) { /* shutting down */ }
            catch (Exception ex)
            {
                _log.LogError(ex, "ReminderWorker fejl i loop");
            }
        }
    }

    private async Task RunOnce(CancellationToken ct)
    {
        // 1) Find events præcis 2 dage frem som ikke er sendt
        var eventsInTwoDays = await _calRepo.FindByExactOffsetDays(2);
        if (eventsInTwoDays.Count == 0)
        {
            _log.LogInformation("Ingen events +2 dage. Intet sendt.");
            return;
        }

        // 2) Modtagere (deduplikeret)
        var recipients = _userRepo.GetAll()
            .Where(u => !string.IsNullOrWhiteSpace(u.Email))
            .Select(u => u.Email!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (recipients.Length == 0)
        {
            _log.LogWarning("Ingen modtagere fundet.");
            return;
        }

        // 3) SMTP config
        var host = _config["Smtp:Host"]!;
        var port = int.Parse(_config["Smtp:Port"] ?? "587");
        var enableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "true");
        var smtpUser = _config["Smtp:User"]!;
        var smtpPass = _config["Smtp:Password"]!;  // fra User Secrets / env
        var from = _config["Smtp:From"] ?? smtpUser;

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            Timeout = 1000 * 30
        };

        int sentTotal = 0;

        foreach (var ev in eventsInTwoDays)
        {
            var dateStr = ev.Date.ToString("dd-MM-yyyy");
            var subject = $"Påmindelse: Whist-holdet {dateStr}";

            // Plain + HTML (bedre leveringsrate)
            var text = $"Husk, du har en begivenhed med Whist-holdet d. {dateStr} om 2 dage."
                     + (string.IsNullOrWhiteSpace(ev.Note) ? "" : $"\n\nNote: {ev.Note}")
                     + "\n\nVi ses!\nWhist-holdet";

            var html = $@"
<p>Husk, du har en begivenhed med Whist-holdet d.d. <strong>{dateStr}</strong> <em>om 2 dage</em>.</p>
{(string.IsNullOrWhiteSpace(ev.Note) ? "" : $"<p><strong>Note:</strong> {System.Net.WebUtility.HtmlEncode(ev.Note)}</p>")}
<hr/>
<p>Vi ses!<br/>Whist-holdet</p>";

            foreach (var to in recipients)
            {
                using var msg = new MailMessage
                {
                    From = new MailAddress(from, "Whist holdet"),
                    Subject = subject,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    HeadersEncoding = Encoding.UTF8
                };
                msg.To.Add(to);
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html));

                try
                {
                    await client.SendMailAsync(msg, ct);
                    sentTotal++;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Kunne ikke sende til {To}", to);
                }
            }

            // Markér som sendt (så vi ikke sender igen)
            await _calRepo.MarkReminderSent(ev.Id);
        }

        _log.LogInformation("ReminderWorker sendte {Sent} mails for {Events} events.", sentTotal, eventsInTwoDays.Count);
    }
}
