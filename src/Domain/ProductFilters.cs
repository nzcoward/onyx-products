namespace Onyx.Products.Domain;

using System.Drawing;

public record ProductFilters(string? Colour = null)
{
    public Color GetColour()
    {
        if(Colour is null)
            return Color.Empty;

        if (Colour.TryGetColor(out var colour))
            return colour.Value;

        return Color.Empty;
    }

    public IQueryable<Product> Apply(IQueryable<Product> query)
    {
        if (string.IsNullOrWhiteSpace(Colour))
            return query;

        var colour = GetColour();

        if (colour.IsEmpty)
            return query;

        return query.Where(x => x.Colour == colour);
    }
}
