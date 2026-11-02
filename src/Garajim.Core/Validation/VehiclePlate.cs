using System.Text.RegularExpressions;

namespace Garajim.Core.Validation;

public static class VehiclePlate
{
    private static readonly Regex TurkishFormat = new(@"^(\d{2})([A-Z]{1,3})(\d{2,4})$", RegexOptions.Compiled);

    /// <summary>Boşluk ve tireleri atar, büyük harfe çevirir: "35 abc 123" → "35ABC123".</summary>
    public static string Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return new string(input.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
    }

    /// <summary>Türk plaka biçimindeyse "35 ABC 123" gibi gösterir, değilse olduğu gibi döner.</summary>
    public static string Format(string normalized)
    {
        var m = TurkishFormat.Match(normalized);
        return m.Success ? $"{m.Groups[1].Value} {m.Groups[2].Value} {m.Groups[3].Value}" : normalized;
    }
}
