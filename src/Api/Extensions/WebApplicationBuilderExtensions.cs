namespace Onyx.Products.Api.Extensions;

using Asp.Versioning;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Onyx.Products.Domain;
using OpenTelemetry.Trace;

using System.Drawing;
using System.Text;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddProductsDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("products");

        builder.Services.AddDbContext<ProductsDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var anyProducts = await context
                        .Set<Product>()
                        .AnyAsync(cancellationToken);

                    if (anyProducts)
                        return;

                    var products = context.Set<Product>();
                    products.Add(Product.Create("Brown Casual Shoes 001", "SHO-CAS-BRO-001", Color.Brown));
                    products.Add(Product.Create("Brown Casual Shoes 002", "SHO-CAS-BRO-002", Color.Brown));
                    products.Add(Product.Create("Green Casual Shoes 001", "SHO-CAS-GRE-001", Color.Green));
                    products.Add(Product.Create("White Casual Shoes 001", "SHO-CAS-WHI-001", Color.White));
                    products.Add(Product.Create("White Casual Shoes 002", "SHO-CAS-WHI-002", Color.White));

                    await context.SaveChangesAsync(cancellationToken);
                });
        });

        return builder;
    }

    public static WebApplicationBuilder ProtectApi(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSecret")!)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                        context.Token = token;
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiAuthorizationPolicy", policy => policy.RequireRole("OnyxAdmin"));
        });

        return builder;
    }

    public static WebApplicationBuilder AddProductsDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IProductsService, ProductsService>();

        return builder;
    }

    public static WebApplicationBuilder AddAllowAllCors(this WebApplicationBuilder builder)
    {
        var allowAllCorsPolicy = "allow-all";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                allowAllCorsPolicy,
                policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
        });

        return builder;
    }

    public static WebApplicationBuilder AddStandardApiVersioning(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");

            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());

        return builder;
    }
}
