namespace Core;

/// <summary>
/// Point-transaktion for en spiller.
/// Bruges til historik og til at beregne total-score over tid.
/// </summary>
public class PointEntry
{
    /// <summary>Unikt ID (genereres i repository).</summary>
    public int Id { get; set; }

    /// <summary>ID på spilleren (User).</summary>
    public int PlayerId { get; set; }

    /// <summary>Point der tildeles (kan være positiv eller negativ).</summary>
    public int Points { get; set; }

    /// <summary>Hvornår pointene blev tildelt.</summary>
    public DateTime Date { get; set; }
}