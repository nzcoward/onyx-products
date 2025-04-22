using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

internal static class ProductEndpoints
{
    private const string root = "products";

    public static void Map(WebApplication app)
    {
        var versionSet = app
            .NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .Build();


        app.MapGet($"/{root}", async (HttpContext context, CancellationToken cancellationToken) =>
        {
            await Task.Delay(100, cancellationToken);
            return Results.Ok(new Product[] { new Product("Product A", "Green") });
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .WithOpenApi()
        .WithName("GetProducts");

        app.MapGet($"/{root}/{{id:guid}}", async (Guid id) =>
        {
            return Results.Ok();
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
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
