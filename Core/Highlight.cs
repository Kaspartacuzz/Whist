using System.ComponentModel.DataAnnotations;

namespace Core;

// Repræsenterer et højdepunkt knyttet til en bruger
public class Highlight
{
    public int Id { get; set; }                             // Unikt ID for highlightet
    [MaxLength(80)] public string Title { get; set; } = "";                 // Kort titel for highlightet
    [MaxLength(400)] public string Description { get; set; } = "";           // Beskrivelse af hvad highlightet handler om
    public DateTime Date { get; set; } = DateTime.Today;    // Dato for highlightet
    public string? ImageUrl { get; set; }                   // Billede uploadet eller taget
    public int UserId { get; set; }                         // ID på den bruger, highlightet tilhører
    public bool IsPrivate { get; set; } = false;            // Bruges til at gøre highlights private
}  