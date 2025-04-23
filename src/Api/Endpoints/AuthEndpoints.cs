using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Onyx.Products.Api.Requests;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

internal static class AuthEndpoints
{
    private const string root = "auth";

    private const string username = "testuser";
    private const string password = "password123";

    public static void Map(WebApplication app)
    {
        var versionSet = app
            .NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .Build();

        var secret = app.Configuration.GetValue<string>("JwtSecret");

        app.MapPost($"/{root}", async ([FromBody] AuthRequest request) =>
        {
            if (request.Username == username && request.Password == password)
            {
                var token = GenerateJwtToken(request.Username, secret);
                return Results.Ok(new { Token = token });
            }

            return Results.Unauthorized();
        })
        .WithApiVersionSet(versionSet)
        .MapToApiVersion(1.0)
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi()
        .WithName("Authenticate");
    }

    private static string GenerateJwtToken(string username, string secret)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "OnyxAdmin")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}