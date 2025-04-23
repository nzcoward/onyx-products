using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Onyx.Products.Api.Requests;
using Onyx.Products.Api.Responses;
using Onyx.Products.Domain;

internal static class ProductsEndpoints
{
    private const string root = "products";

    public static void Map(WebApplication app)
    {
        var versionSet = app
            .NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .Build();


        app.MapGet($"/{root}", async (HttpContext context, [FromServices] IProductsService productsService, CancellationToken cancellationToken) =>
        {
            var productsList = await productsService.GetProductsAsync(cancellationToken);
            return Results.Ok(productsList.ToResponse());
        })
        .RequireAuthorization("ApiAuthorizationPolicy")
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces<ProductsResponse>(StatusCodes.Status200OK)
        .WithOpenApi()
        .WithName("GetProducts");

        app.MapGet($"/{root}/{{sku}}", async (string sku, [FromServices] IProductsService productsService, CancellationToken cancellationToken) =>
        {
            var product = await productsService.GetProductBySkuAsync(sku, cancellationToken);

            if (product is null)
                return Results.NotFound(); // We could use problem details, but I think if a public API, 404 is better; if private would consider a 400.

            return Results.Ok(product.ToResponse());
        })
        .RequireAuthorization("ApiAuthorizationPolicy")
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces<ProductResponse>(StatusCodes.Status200OK)
        .WithOpenApi()
        .WithName("GetProductBySku");

        app.MapPost($"/{root}", async ([FromBody] ProductRequest request, [FromServices] IProductsService productsService, CancellationToken cancellationToken) =>
        {
            var validationResult = request.Validate();

            if(!validationResult.IsValid)
                return Results.Problem(
                    title: "Supplied product details do not meet standard.",
                    statusCode: StatusCodes.Status400BadRequest);

            var product = Product.Create(request.Name, request.Sku, request.GetColour());
            await productsService.CreateProductAsync(product, cancellationToken);

            // We could return Accepted and have a managed async/long running process too offload, well, load.
            return Results.Created($"/{root}/{product.Sku}", product.ToResponse());
        })
        .RequireAuthorization("ApiAuthorizationPolicy")
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces<ProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
        .WithOpenApi()
        .WithName("CreateProduct");
    }
}

public static class ProductExtensions
{
    public static ProductResponse ToResponse(this Product product)
        => new ProductResponse(product.Name, product.Sku, product.Colour.ToHex());

    public static ProductsResponse ToResponse(this IEnumerable<Product> products)
        => new ProductsResponse(products.Select(ToResponse));
}
