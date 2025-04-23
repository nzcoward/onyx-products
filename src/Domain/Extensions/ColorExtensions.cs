using System.Diagnostics.CodeAnalysis;
using System.Drawing;

public static class ColorExtensions
{
    public static String ToHex(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public static Color FromHex(this string hex)
    {
        var result = ColorTranslator.FromHtml(hex);

        if (!result.IsKnownColor)
            return result;

        var knownColor = result.ToKnownColor();
        return Color.FromKnownColor(knownColor);
    }

    public static bool TryGetColor(this string input, [NotNullWhen(true)] out Color? colour)
    {
        try
        {
            colour = input switch
            {
                "" => Color.Empty,
                null => Color.Empty,
                var hex when hex.FirstOrDefault() is '#' => hex.FromHex(), // We assume anything starting with # is a valid hex colour
                var name when Color.FromName(name).IsKnownColor => Color.FromName(name), // Known color name is nice...
                _ => Color.Empty // Not a known color name.
            };

            return true;
        }
        catch
        {
            colour = null;
            return false;
        }
    }
}