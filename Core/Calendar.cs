namespace Core;

public class Calendar
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; } = "";
    public bool ReminderSent { get; set; } = false;
}
