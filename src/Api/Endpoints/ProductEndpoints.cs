using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Onyx.Products.Api.Requests;
using Onyx.Products.Domain.Services;

internal static class ProductEndpoints
{
    private const string root = "products";

    public static void Map(WebApplication app)
    {
        var versionSet = app
            .NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .Build();


        app.MapGet($"/{root}", async (HttpContext context, [FromServices]IProductsService productsService, CancellationToken cancellationToken) =>
        {
            var productsList = await productsService.GetProductsAsync(cancellationToken);
            //return Results.Ok(new Product[] { new Product("Product A", "Green") });
            return Results.Ok(productsList);
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces<List<Product>>(StatusCodes.Status200OK)
        .WithOpenApi()
        .WithName("GetProducts");

        app.MapGet($"/{root}/{{id:guid}}", async (Guid id, [FromServices] IProductsService productsService, CancellationToken cancellationToken) =>
        {
            var product = await productsService.GetProductsAsync(cancellationToken);
            return Results.Ok(product);
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces<Product>(StatusCodes.Status200OK)
        .WithOpenApi()
        .WithName("GetProductById");

        app.MapPost($"/{root}", async ([FromBody] ProductRequest request) =>
        {
            var id = Guid.NewGuid();
            return Results.Accepted($"/products/{id}", request);
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces<List<Product>>(StatusCodes.Status201Created)
        .WithOpenApi()
        .WithName("CreateProduct");
    }
}
