namespace Core;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string NickName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}