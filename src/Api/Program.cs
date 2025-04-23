using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Onyx.Products.Api.Extensions;
using Onyx.Products.Domain;

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
    .AddCheck("Api", () => HealthCheckResult.Healthy("The application is healthy."))
    .AddNpgSql
    (
        builder.Configuration.GetConnectionString("products")!,
        name: "Db",
        failureStatus: HealthStatus.Degraded
    );

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

// Ensure any unhandled exceptions still use the ProblemDetails format.
app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context => await Results.Problem().ExecuteAsync(context)));

app.UseHttpsRedirection();
app.UseAllowAllCors();
app.UseAuthentication();
app.UseAuthorization();

AuthEndpoints.Map(app);
ProductsEndpoints.Map(app);

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            })
        };

        await context.Response.WriteAsJsonAsync(response);
    }
});

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
