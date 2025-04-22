public class ProductTests
{
    [Test]
    public async Task Test1()
    {
        // Arrange
        var product = new Product("Test Product", 10.0m);

        // Act
        var result = product.GetProductDetails();

        // Assert
        Assert.Equal("Test Product - $10.00", result);
    }
}