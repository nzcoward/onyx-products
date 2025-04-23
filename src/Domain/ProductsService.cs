namespace Onyx.Products.Domain;

using Microsoft.EntityFrameworkCore;

public interface IProductsService
{
    Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken);
    Task<List<Product>> GetProductsAsync(ProductFilters filters, CancellationToken cancellationToken);
    Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken);
}

public class ProductsService : IProductsService
{
    private readonly ProductsDbContext _context;
    public ProductsService(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);

        return product;
    }

    public async Task<List<Product>> GetProductsAsync(ProductFilters filters, CancellationToken cancellationToken)
    {
        var products = _context.Products;
        var query = filters.Apply(products);

        return await query
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(x => x.Sku == sku, cancellationToken);
        return product;
    }
}
