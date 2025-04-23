using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Onyx.Products.Api.Requests;

internal static class AuthEndpoints
{
    private const string root = "auth";

    public static void Map(WebApplication app)
    {
        var versionSet = app
            .NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .Build();

        app.MapPost($"/{root}", async ([FromBody] AuthRequest request) =>
        {
            
            return Results.Accepted($"/products/{id}", request);
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces<List<Product>>(StatusCodes.Status201Created)
        .WithOpenApi()
        .WithName("Authenticate");
    }
}