using System.Drawing;

public class ColorConversionTests
{
    [Test]
    public async Task WhenInputIsValidHexThenValidColor()
    {
        var expectedColor = Color.Brown;

        var hex = "#A52A2A";
        hex.TryGetColor(out var result);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Value.A).IsEqualTo(expectedColor.A);
        await Assert.That(result!.Value.R).IsEqualTo(expectedColor.R);
        await Assert.That(result!.Value.G).IsEqualTo(expectedColor.G);
        await Assert.That(result!.Value.B).IsEqualTo(expectedColor.B);
    }

    [Test]
    public async Task WhenInputIsValidNameThenValidColor()
    {
        var expectedColor = Color.Brown;

        var name = "Brown";
        name.TryGetColor(out var result);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(expectedColor);
    }

    [Test]
    public async Task WhenInputIsInvalidNameThenEmptyColor()
    {
        var expectedColor = Color.Empty;

        var name = "FAKECOLOR";
        name.TryGetColor(out var result);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(expectedColor);
    }

    [Test]
    public async Task WhenInputIsColorThenValidHex()
    {
        var expectedHex = "#A52A2A";

        var color = Color.Brown;
        var result = color.ToHex();

        await Assert.That(result).IsEqualTo(expectedHex);
    }
}