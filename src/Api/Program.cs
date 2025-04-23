using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

using Onyx.Products.Api.Extensions;
using Onyx.Products.Domain;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.AddProductsDomain();
builder.AddProductsDbContext();

builder.AddAllowAllCors();
builder.AddStandardApiVersioning();

builder.ProtectApi();

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
        options.EnablePersistAuthorization();
    });

    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context
        => await Results.Problem().ExecuteAsync(context)));

app.UseHttpsRedirection();
app.UseAllowAllCors();
app.UseAuthentication();
app.UseAuthorization();

AuthEndpoints.Map(app);
ProductsEndpoints.Map(app);

app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
