using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {

            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // "bearer" refers to the header name here
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            };

            var bearerRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                }] = Array.Empty<string>()
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = bearerScheme
            };

            var paths = document.Paths.Where(x => !x.Key.Contains("auth")).ToList();

            foreach (var operation in paths.SelectMany(path => path.Value.Operations))
            {
                operation.Value.Security.Add(bearerRequirement);
            }

            //document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
            //document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            //{
            //    { bearerScheme, [] }
            //});
        }
    }
}