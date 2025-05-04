namespace Core;

// Repræsenterer en bruger i systemet
public class User
{
    public int Id { get; set; }                                             // Unikt ID for brugeren
    public string Name { get; set; } = "";                                  // Brugerens fulde navn
    public string NickName { get; set; } = "";                              // Brugerens kaldenavn (visningsnavn)
    public string Email { get; set; } = "";                                 // Brugerens e-mailadresse
    public string Password { get; set; } = "";                              // Brugerens adgangskode (skal senere hashes i rigtig database)
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();        // Liste over bøder
}