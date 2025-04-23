using System.Diagnostics.CodeAnalysis;
using System.Drawing;

public static class ColorExtensions
{
    public static String ToHex(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public static bool TryGetColor(this string input, [NotNullWhen(true)] out Color? colour)
    {
        try
        {
            colour = input switch
            {
                "" => Color.Empty,
                null => Color.Empty,
                var hex when hex.FirstOrDefault() is '#' => ColorTranslator.FromHtml(hex), // We assume anything starting with # is a valid hex colour
                _ => Color.FromName(input) // Else we assume a name
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