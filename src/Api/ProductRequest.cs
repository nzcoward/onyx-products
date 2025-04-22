using System.Diagnostics.CodeAnalysis;
using System.Drawing;

internal record ProductRequest(string Name, string Colour) : IValidateable
{
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
        {
            errors.Add("Name must be a valid name.");
        }

        if (string.IsNullOrWhiteSpace(Colour))
        {
            if(!TryGetColor(Colour, out var _))
            {
                errors.Add("Colour is not a valid colour.");
            }
        }

        return new ValidationResult(!errors.Any(), errors.ToArray());
    }

    private static bool TryGetColor(string input, [NotNullWhen(true)]out Color? colour)
    {
        try
        {
            colour = input switch
            {
                ['#', _] => ColorTranslator.FromHtml(input), // We assume this is a hex colour
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
