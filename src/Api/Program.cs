using Asp.Versioning;

using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.ApiVersionReader = new HeaderApiVersionReader("x-onyxapi-version");

    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddHealthChecks()
    .AddCheck("ApiHealthy", () =>
        HealthCheckResult.Healthy("The application is healthy."));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(allowAllCorsPolicy);

ProductEndpoints.Map(app);

app.MapHealthChecks("/health");

app.Run();