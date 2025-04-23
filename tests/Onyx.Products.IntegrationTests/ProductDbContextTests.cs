using Onyx.Products.Domain;

using System.Drawing;

public class ProductDbContextTests
{
    // We might want to consider (x-unit) theory-style parameterised tests (still uses [Test] in TUnit.
    [Test]
    public async Task When5ProductsAddedThen5ResultsReturned()
    {
        var expectedCount = 5;

        await using var context = new MockDb().CreateDbContext();

        var products = context.Set<Product>();
        products.Add(Product.Create("Brown Casual Shoes 001", "SHO-CAS-BRO-001", Color.Brown));
        products.Add(Product.Create("Brown Casual Shoes 002", "SHO-CAS-BRO-002", Color.Brown));
        products.Add(Product.Create("Green Casual Shoes 001", "SHO-CAS-GRE-001", Color.Green));
        products.Add(Product.Create("White Casual Shoes 001", "SHO-CAS-WHI-001", Color.White));
        products.Add(Product.Create("White Casual Shoes 002", "SHO-CAS-WHI-002", Color.White));

        await context.SaveChangesAsync();

        var service = new ProductsService(context);

        var results = await service.GetProductsAsync(new ProductFilters(), CancellationToken.None);

        await Assert.That(results).HasCount(expectedCount);
    }

    [Test]
    public async Task WhenNoProductsAddedThenNoResultsReturned()
    {
        var expectedCount = 0;

        await using var context = new MockDb().CreateDbContext();

        var products = context.Set<Product>();

        await context.SaveChangesAsync();

        var service = new ProductsService(context);

        var results = await service.GetProductsAsync(new ProductFilters(), CancellationToken.None);

        await Assert.That(results).HasCount(expectedCount);
    }

    [Test]
    public async Task WhenValidSkuThenSingleItemReturned()
    {
        var sku = "SHO-CAS-GRE-001";

        await using var context = new MockDb().CreateDbContext();

        var products = context.Set<Product>();
        products.Add(Product.Create("Brown Casual Shoes 001", "SHO-CAS-BRO-001", Color.Brown));
        products.Add(Product.Create("Green Casual Shoes 001", "SHO-CAS-GRE-001", Color.Green));
        products.Add(Product.Create("White Casual Shoes 001", "SHO-CAS-WHI-001", Color.White));

        await context.SaveChangesAsync();

        var service = new ProductsService(context);

        var result = await service.GetProductBySkuAsync(sku, CancellationToken.None);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Sku).IsEqualTo(sku);
    }

    [Test]
    public async Task WhenInvalidSkuThenNullItemReturned()
    {
        var sku = "FAKESKU";

        await using var context = new MockDb().CreateDbContext();

        var products = context.Set<Product>();
        products.Add(Product.Create("Brown Casual Shoes 001", "SHO-CAS-BRO-001", Color.Brown));

        await context.SaveChangesAsync();

        var service = new ProductsService(context);

        var result = await service.GetProductBySkuAsync(sku, CancellationToken.None);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task When2BrownProductsAndRequestingBrownItemsThen2ResultsReturned()
    {
        var colour = Color.Brown;
        var expectedCount = 2;

        await using var context = new MockDb().CreateDbContext();

        var products = context.Set<Product>();
        products.Add(Product.Create("Brown Casual Shoes 001", "SHO-CAS-BRO-001", Color.Brown));
        products.Add(Product.Create("Brown Casual Shoes 002", "SHO-CAS-BRO-002", Color.Brown));
        products.Add(Product.Create("Green Casual Shoes 001", "SHO-CAS-GRE-001", Color.Green));
        products.Add(Product.Create("White Casual Shoes 001", "SHO-CAS-WHI-001", Color.White));

        await context.SaveChangesAsync();

        var service = new ProductsService(context);

        var results = await service.GetProductsAsync(new ProductFilters(colour.ToHex()), CancellationToken.None);

        await Assert.That(results).HasCount(expectedCount);
        await Assert.That(results).All().Satisfy(x => x.Colour, color => color.IsEqualTo(colour));
    }

    [Test]
    public async Task When1GreenProductAndRequestingGreenProductsThen1ResultReturned()
    {
        var colour = Color.Green;
        var expectedCount = 1;

        await using var context = new MockDb().CreateDbContext();

        var products = context.Set<Product>();
        products.Add(Product.Create("Brown Casual Shoes 001", "SHO-CAS-BRO-001", Color.Brown));
        products.Add(Product.Create("Green Casual Shoes 001", "SHO-CAS-GRE-001", Color.Green));
        products.Add(Product.Create("White Casual Shoes 001", "SHO-CAS-WHI-001", Color.White));

        await context.SaveChangesAsync();

        var service = new ProductsService(context);

        var results = await service.GetProductsAsync(new ProductFilters(colour.ToHex()), CancellationToken.None);

        await Assert.That(results).HasCount(expectedCount);
        await Assert.That(results).All().Satisfy(x => x.Colour, color => color.IsEqualTo(colour));
    }
}
