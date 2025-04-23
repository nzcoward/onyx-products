using Microsoft.EntityFrameworkCore;

using Onyx.Products.Domain;

public class MockDb : IDbContextFactory<ProductsDbContext>
{
    public ProductsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new ProductsDbContext(options);
    }
}