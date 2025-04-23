namespace Onyx.Products.Api.Requests;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

internal record ProductRequest(string Name, string Sku, string Colour) : IValidateable
{
    public Color GetColour()
    {
        if (TryGetColor(Colour, out var colour))
            return colour.Value;

        return Color.Empty;
    }

    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name must be a valid name.");

        if (string.IsNullOrWhiteSpace(Sku))
            errors.Add("Sku must be valid.");

        if (string.IsNullOrWhiteSpace(Colour))
        {
            if(!TryGetColor(Colour, out var _))
                errors.Add("Colour must be a valid colour.");
        }

        return new ValidationResult(!errors.Any(), errors.ToArray());
    }

    private static bool TryGetColor(string input, [NotNullWhen(true)]out Color? colour)
    {
        try
        {
            colour = input switch
            {
                ['#', _] => ColorTranslator.FromHtml(input), // We assume anything starting with # is a valid hex colour
                "" => Color.Empty,
                null => Color.Empty,
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
