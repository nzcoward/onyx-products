using System.Drawing;

public class Product
{
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public Color Colour { get; private set; }

    private Product(string name, string sku, Color colour)
    {
        Name = name;
        Sku = sku;
        Colour = colour;
    }

    public static Product Create(string name, string sku, Color colour)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must be a valid name.", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU must be a valid SKU.", nameof(sku));

        return new Product(name, sku, colour);
    }
}
