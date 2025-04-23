using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Onyx.Products.Api.Extensions;
using Onyx.Products.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.AddProductsDomain();
builder.AddProductsDbContext();

builder.AddAllowAllCors();
builder.AddStandardApiVersioning();

builder.Services
    .AddAuthentication()
    .AddJwtBearer();

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("ApiTesterPolicy", b => b.RequireRole("tester"));
});

var requireAuthPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddHealthChecks()
    .AddCheck("ApiHealthy", () =>
        HealthCheckResult.Healthy("The application is healthy."));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });

    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context
        => await Results.Problem().ExecuteAsync(context)));

app.UseHttpsRedirection();
app.UseAllowAllCors();

ProductsEndpoints.Map(app);

app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();