namespace Onyx.Products.Domain.Services;

using System.Collections.Concurrent;
using System.Drawing;

public interface IProductsService
{
    Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken);
    Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken);
    Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Product>> GetProductsByColorAsync(Color color, CancellationToken cancellationToken);
}

public class ProductsService : IProductsService
{
    private ConcurrentDictionary<Guid, Product> _products = new();

    public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        _products.TryAdd(id, product);

        return product;

    }

    public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return _products.Values.ToList();
    }

    public async Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if(!_products.TryGetValue(id, out var product))
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        return product;
    }

    public async Task<List<Product>> GetProductsByColorAsync(Color color, CancellationToken cancellationToken)
    {
        return _products.Values
            .Where(x => x.Colour == color)
            .ToList();
    }
}
