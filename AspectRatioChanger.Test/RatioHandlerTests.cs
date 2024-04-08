using AspectRatioChanger.Pocos;

namespace AspectRatioChanger.Test;

public class RatioHandlerTests
{
    private readonly RatioHandler _sut;

    public RatioHandlerTests()
    {
        _sut = new RatioHandler();
    }

    [Theory]
    [InlineData(16, 9)]
    [InlineData(21, 9)]
    [InlineData(2, 1)]
    [InlineData(16, 8)]
    public void Widescreen_aspect_ratio_should_not_stretch(int width, int height)
    {
        // Arrange
        var testData = new List<VideoRoot> { new() { aspect_w = width, aspect_h = height } };

        // Act
        var dockedModes = _sut.AddDockedModes(testData, 1.2);

        // Assert
        Assert.Null(dockedModes[0].dock_aspect_w);
        Assert.Null(dockedModes[0].dock_aspect_h);
    }

    [Theory]
    [InlineData(16, 10)]
    [InlineData(5, 4)]
    [InlineData(4, 3)]
    [InlineData(8, 7)]
    public void Widescreen_aspect_ratio_should_be_should_stretch(int width, int height)
    {
        // Arrange
        var testData = new List<VideoRoot> { new() { aspect_w = width, aspect_h = height } };

        // Act
        var dockedModes = _sut.AddDockedModes(testData, 1.1);

        // Assert
        Assert.NotNull(dockedModes[0].dock_aspect_w);
        Assert.NotNull(dockedModes[0].dock_aspect_h);
    }

    [Fact]
    public void Widescreen_aspect_ratio_should_never_be_over_stretched()
    {
        // Arrange
        var testData = new List<VideoRoot> { new() { aspect_w = 16, aspect_h = 10 } };

        // Act
        var dockedModes = _sut.AddDockedModes(testData, 1.5);

        // Assert
        Assert.Equal(16, dockedModes[0].dock_aspect_w);
        Assert.Equal(9, dockedModes[0].dock_aspect_h);
    }

    [Fact]
    public void Reset_flag_should_set_null()
    {
        // Arrange
        var testData = new List<VideoRoot> { new() { dock_aspect_w = 16, dock_aspect_h = 10 } };

        // Act
        var dockedModes = _sut.AddDockedModes(testData, 1.1, true);

        // Assert
        Assert.Null(dockedModes[0].dock_aspect_w);
        Assert.Null(dockedModes[0].dock_aspect_h);
    }
}