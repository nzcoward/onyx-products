using System.Drawing;

public class ProductTests
{
    [Test]
    public async Task Test1()
    {
        // Arrange
        var product = Product.Create("Test Product", "SKU", Color.Red);

        // Act
        // var result = product.GetProductDetails();

        // Assert
        // Assert.Equal("Test Product - $10.00", result);
    }
}