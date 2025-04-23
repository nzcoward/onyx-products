namespace Onyx.Products.Domain;

using Microsoft.EntityFrameworkCore;
using System.Drawing;

public interface IProductsService
{
    Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken);
    Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken);
    Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken);
    Task<List<Product>> GetProductsByColorAsync(Color color, CancellationToken cancellationToken);
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

    public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(x => x.Sku == sku, cancellationToken);
        return product;
    }

    public async Task<List<Product>> GetProductsByColorAsync(Color color, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(x => x.Colour == color)
            .ToListAsync(cancellationToken);
    }
}
