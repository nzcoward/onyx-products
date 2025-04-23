namespace Onyx.Products.Api.Responses;

public record ProductsResponse(IEnumerable<ProductResponse> Products);

public record ProductResponse(string Name, string Sku, string Colour);
