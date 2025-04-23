namespace Onyx.Products.Domain;

using Microsoft.EntityFrameworkCore;
using System.Drawing;

public class ProductsDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var productBuilder = modelBuilder.Entity<Product>();

        // We are going to use a shadow property for the Id as we should be using e.g. sku for uniqueness
        productBuilder
            .Property<int>("_id");

        productBuilder
            .HasKey("_id");

        productBuilder
            .HasIndex(p => p.Sku)
            .IsUnique();

        productBuilder
            .Property(p => p.Colour)
            .HasConversion(
                v => v.ToHex(),
                v => ColorTranslator.FromHtml(v));
    }
}
