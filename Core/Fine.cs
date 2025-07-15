using System.ComponentModel.DataAnnotations;

namespace Core;

// Repræsenterer en bøde givet til en bruger
public class Fine
{
    public int Id { get; set; }                 // Unikt ID for bøden
    public int UserId { get; set; }             // ID på den bruger, bøden tilhører
    [Range(1, 500)] public decimal Amount { get; set; }         // Beløb for bøden
    [MaxLength(400)] public string Comment { get; set; } = "";   // Valgfri kommentar til bøden
    public DateTime Date { get; set; }          // Hvornår bøden blev givet
    public bool IsPaid { get; set; } = false;   // Om bøden er betalt
    public string FineType { get; set; } = "Almindelig"; // "Almindelig" eller "Stjerne"
}