using System.ComponentModel.DataAnnotations;

namespace Core;

public class Rule
{
    public int Id { get; set; } 
    
    [MaxLength(500)] public string Text { get; set; } = string.Empty;
}