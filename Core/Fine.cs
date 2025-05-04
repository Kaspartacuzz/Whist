namespace Core;

public class Fine
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Comment { get; set; } = "";
    public DateTime Date { get; set; }
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidDate { get; set; }
    public User? User { get; set; } // Navigation property
}