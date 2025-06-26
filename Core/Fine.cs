namespace Core;

// Repræsenterer en bøde givet til en bruger
public class Fine
{
    public int Id { get; set; }                 // Unikt ID for bøden
    public int UserId { get; set; }             // ID på den bruger, bøden tilhører
    public decimal Amount { get; set; }         // Beløb for bøden
    public string Comment { get; set; } = "";   // Valgfri kommentar til bøden
    public DateTime Date { get; set; }          // Hvornår bøden blev givet
    public bool IsPaid { get; set; } = false;   // Om bøden er betalt
    public DateTime? PaidDate { get; set; }     // Dato for betaling, hvis betalt
    public string FineType { get; set; } = "Almindelig"; // "Almindelig" eller "Stjerne"
    public User? User { get; set; }             // Navigation property til brugerobjektet
}