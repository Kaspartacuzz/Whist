namespace Core;

// Repræsenterer et højdepunkt knyttet til en bruger
public class Highlight
{
    public int Id { get; set; }                             // Unikt ID for highlightet
    public string Title { get; set; } = "";                 // Kort titel for highlightet
    public string Description { get; set; } = "";           // Beskrivelse af hvad highlightet handler om
    public DateTime Date { get; set; } = DateTime.Today;    // Dato for highlightet
    public int UserId { get; set; }                         // ID på den bruger, highlightet tilhører
    public User? User { get; set; }                         // Navigation property til brugerobjektet
}  