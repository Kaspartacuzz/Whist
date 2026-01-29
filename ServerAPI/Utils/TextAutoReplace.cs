using System.Text.RegularExpressions;
using Core;

namespace ServerAPI.Utils;

/// <summary>
/// Centralt sted til interne "text rules".
/// Lige nu: auto-udskift KSDH (case-insensitive) => BIF&lt;3
/// </summary>
public static class TextAutoReplace
{
    private static readonly Regex KsdhRegex = new("KSDH", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>Erstatter KSDH/ksdh/uanset casing med BIF&lt;3.</summary>
    public static string? Apply(string? input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return KsdhRegex.Replace(input, "BIF<3");
    }

    // --- Model-specifik helpers (så repos forbliver simple) ---

    public static void Apply(User user)
    {
        // OBS: Jeg ændrer IKKE Email/Password, da det kan ødelægge login.
        // Hvis du vil have det med, siger du til.
        user.Name = Apply(user.Name) ?? "";
        user.NickName = Apply(user.NickName) ?? "";
        user.Address = Apply(user.Address) ?? "";
        user.Description = Apply(user.Description) ?? "";
        user.FunFact = Apply(user.FunFact) ?? "";
        // ImageUrl lader vi være.
    }

    public static void Apply(Fine fine)
    {
        fine.Comment = Apply(fine.Comment) ?? "";
    }

    public static void Apply(Highlight highlight)
    {
        highlight.Title = Apply(highlight.Title) ?? "";
        highlight.Description = Apply(highlight.Description) ?? "";
        // ImageUrl lader vi være.
    }

    public static void Apply(Calendar calendar)
    {
        calendar.Note = Apply(calendar.Note) ?? "";
    }

    public static void Apply(Rule rule)
    {
        rule.Text = Apply(rule.Text) ?? "";
    }
}