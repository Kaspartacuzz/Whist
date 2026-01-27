using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerAPI.Repositories;
using ServerAPI.Repositories.Calendars;

namespace ServerAPI.Workers;

/// <summary>
/// Worker der sender mails:
/// A) Fødselsdage (i dag)
/// B) Kalender events (+2 dage)
///
/// Kører 1 gang dagligt kl. 09:00 lokal tid.
/// </summary>
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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Kør dagligt kl. 09:00 lokal tid
                var now = DateTime.Now;
                var nextRun = now.Date.AddHours(9);

                if (now >= nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                _log.LogInformation("ReminderWorker næste kørsel: {NextRun} (om {Delay})", nextRun, delay);

                await Task.Delay(delay, stoppingToken);
                await RunOnce(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "ReminderWorker fejl i loop");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task RunOnce(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        // 1) Hent modtagere (bruges til både fødselsdage og events)
        var users = _userRepo.GetAll()
            .Where(u => !string.IsNullOrWhiteSpace(u.Email))
            .ToList();

        if (users.Count == 0)
        {
            _log.LogWarning("Ingen modtagere fundet.");
            return;
        }

        // 2) SMTP config
        var host = _config["Smtp:Host"]!;
        var port = int.Parse(_config["Smtp:Port"] ?? "587");
        var enableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "true");
        var smtpUser = _config["Smtp:User"]!;
        var smtpPass = _config["Smtp:Password"]!; // fra secrets / env
        var from = _config["Smtp:From"] ?? smtpUser;

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            Timeout = 1000 * 30
        };

        var sentTotal = 0;

        // --------------------------
        // A) Fødselsdage (i dag)
        // --------------------------
        static bool IsBirthdayToday(DateOnly? birthDate, DateOnly todayLocal)
            => birthDate.HasValue && birthDate.Value.Day == todayLocal.Day && birthDate.Value.Month == todayLocal.Month;

        var birthdayUsers = users.Where(u => IsBirthdayToday(u.BirthDate, today)).ToList();

        foreach (var user in birthdayUsers)
        {
            var to = user.Email!.Trim();
            var name = !string.IsNullOrWhiteSpace(user.Name) ? user.Name : user.NickName;

            var subject = "Tillykke med fødselsdagen 🎉";

            var text = $@"Kære {name}

Hjerteligt tillykke med fødselsdagen 🇩🇰
Du ønskes en god dag fra hele Whist holdet

/ Whist holdet";

            var html = $@"
<p>Kære <strong>{WebUtility.HtmlEncode(name)}</strong></p>
<p>Hjerteligt tillykke med fødselsdagen 🇩🇰</p>
<p>Du ønskes en god dag fra hele Whist holdet</p>
<br/>
<p>/ Whist holdet</p>";

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
                _log.LogInformation("Sendte fødselsdagshilsen til {To}", to);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Kunne ikke sende fødselsdagshilsen til {To}", to);
            }
        }

        // --------------------------
        // B) Event reminders (+2 dage)
        // --------------------------
        var eventsInTwoDays = await _calRepo.FindByExactOffsetDays(2);

        if (eventsInTwoDays.Count == 0)
        {
            _log.LogInformation("Ingen events +2 dage. (Fødselsdage kan stadig være sendt).");
            _log.LogInformation("ReminderWorker sendte {Sent} mails (inkl. evt. fødselsdage).", sentTotal);
            return;
        }

        // Deduplikeret recipient-liste
        var recipients = users
            .Select(u => u.Email!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (recipients.Length == 0)
        {
            _log.LogWarning("Ingen modtagere fundet til event reminders.");
            _log.LogInformation("ReminderWorker sendte {Sent} mails (inkl. evt. fødselsdage).", sentTotal);
            return;
        }

        foreach (var ev in eventsInTwoDays)
        {
            var dateStr = ev.Date.ToString("dd-MM-yyyy");
            var subject = $"Påmindelse: Whist-holdet {dateStr}";

            var text =
                $"Husk, du har en begivenhed med Whist-holdet d. {dateStr} om 2 dage."
                + (string.IsNullOrWhiteSpace(ev.Note) ? "" : $"\n\nNote: {ev.Note}")
                + "\n\nVi ses!\nWhist-holdet";

            var html = $@"
<p>Husk, du har en begivenhed med Whist-holdet d.d. <strong>{dateStr}</strong> <em>om 2 dage</em>.</p>
{(string.IsNullOrWhiteSpace(ev.Note) ? "" : $"<p><strong>Note:</strong> {WebUtility.HtmlEncode(ev.Note)}</p>")}
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
        }

        // Markér alle events som sendt i ét batch-kald (bedre performance)
        await _calRepo.MarkRemindersSent(eventsInTwoDays.Select(e => e.Id));

        _log.LogInformation(
            "ReminderWorker sendte {Sent} mails for {Events} events. (Inkl. evt. fødselsdage).",
            sentTotal,
            eventsInTwoDays.Count
        );
    }
}
