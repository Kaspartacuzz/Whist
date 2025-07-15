using System.ComponentModel.DataAnnotations;

namespace Core;

// Repræsenterer en bruger i systemet
public class User
{
    public int Id { get; set; }                                             // Unikt ID for brugeren
    [MaxLength(40)] public string Name { get; set; } = "";                                  // Brugerens fulde navn
    [MaxLength(30)] public string NickName { get; set; } = "";                              // Brugerens kaldenavn (visningsnavn)
    [MaxLength(100)] [EmailAddress] public string Email { get; set; } = "";                                 // Brugerens e-mailadresse
    [MaxLength(100)] public string Password { get; set; } = "";                              // Brugerens adgangskode (skal senere hashes i rigtig database)
    [MaxLength(8)] [MinLength(8)] [Phone]public string PhoneNumber { get; set; } = "";                           // Brugerens Telefonnummer - bruges til mobilepay
    [MaxLength(500)] public string Description { get; set; } = "";                           // Lang tekst om personen
    [MaxLength(200)] public string FunFact { get; set; } = "";                               // Sjov info
    public string ImageUrl { get; set; } = "";                              // Profilbillede
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();        // Liste over bøder
}