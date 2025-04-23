namespace Onyx.Products.Api.Requests;

using System.Drawing;

internal record ProductRequest(string Name, string Sku, string Colour) : IValidateable
{
    public Color GetColour()
    {
        if (Colour.TryGetColor(out var colour))
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
            if(!Colour.TryGetColor(out var _))
                errors.Add("Colour must be a valid colour.");
        }

        return new ValidationResult(!errors.Any(), errors.ToArray());
    }
}
