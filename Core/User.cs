namespace Core;

// Repræsenterer en bruger i systemet
public class User
{
    public int Id { get; set; }                                             // Unikt ID for brugeren
    public string Name { get; set; } = "";                                  // Brugerens fulde navn
    public string NickName { get; set; } = "";                              // Brugerens kaldenavn (visningsnavn)
    public string Email { get; set; } = "";                                 // Brugerens e-mailadresse
    public string Password { get; set; } = "";                              // Brugerens adgangskode (skal senere hashes i rigtig database)
    public string PhoneNumber { get; set; } = "";                           // Brugerens Telefonnummer - bruges til mobilepay
    public string Description { get; set; } = "";                           // Lang tekst om personen
    public string FunFact { get; set; } = "";                               // Sjov info
    public string ImageUrl { get; set; } = "";                              // Profilbillede
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();        // Liste over bøder
}