namespace Core;

public class PointEntry
{
    public int Id { get; set; }              // Unikt ID, genereres automatisk i repo
    public int PlayerId { get; set; }        // Henvisning til User/Member
    public int Points { get; set; }          // Positive eller negative point
    public DateTime Date { get; set; }       // Hvornår pointene blev tildelt
}