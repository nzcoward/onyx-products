namespace Onyx.Products.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseAllowAllCors(this WebApplication app)
    {
        app.UseCors("allow-all");
        return app;
    }
}