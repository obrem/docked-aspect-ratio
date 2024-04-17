using AspectRatioChanger.Handlers;
using AspectRatioChanger.Pocos;

namespace AspectRatioChanger.Test;

public class RatioHandlerTests
{
    private readonly RatioHandler _sut = new();

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

    [Fact]
    public void GetScaledPercentage_should_calculate_correct_percentage()
    {
        // Arrange
        var testData = new VideoRoot
        {
            aspect_w = 140,
            aspect_h = 90,
            dock_aspect_w = 154, 
            dock_aspect_h = 90
        };

        // Act
        var scaledPercentage = _sut.GetScaledPercentage(testData);

        Assert.Equal(110, scaledPercentage);
    }

    [Fact]
    public void Strange_GetScaledPercentage_should_calculate_correct_percentage()
    {
        // Arrange
        var testData = new VideoRoot
        {
            aspect_w = 50,
            aspect_h = 30,
            dock_aspect_w = 55,
            dock_aspect_h = 30
        };

        // Act
        var scaledPercentage = _sut.GetScaledPercentage(testData);

        Assert.Equal(110, scaledPercentage);
    }

    [Theory]
    [InlineData(16, 10)]
    [InlineData(4, 3)]
    [InlineData(8, 7)]
    [InlineData(5, 4)]
    public void Widescreen_aspect_ratio_should_be_stretched_correctly_unless_overstretch(int width, int height)
    {
        // Arrange
        var testData = new List<VideoRoot> { new() { aspect_w = width, aspect_h = height } };


        for (int i = 101; i < 150; i++)
        {
            // Act
            double precentageDecimal = (double)i/ 100;
            var dockedModes = _sut.AddDockedModes(testData, precentageDecimal);
            
            // Assert
            var scaledPercentage = _sut.GetScaledPercentage(dockedModes[0]);
            Assert.Equal(i, scaledPercentage);
        }
       
    }


    [Theory]
    [InlineData(5, 3)]
    [InlineData(21, 10)]
    [InlineData(16, 7)]
    [InlineData(2, 1)]
    public void Widescreen_aspect_ratio_should_be_not_overstretch(int width, int height)
    {
        // Arrange
        var testData = new List<VideoRoot> { new() { aspect_w = width, aspect_h = height } };

        // Act
        var dockedModes = _sut.AddDockedModes(testData, 1.1);

        // Assert
        var scaledPercentage = _sut.GetScaledPercentage(dockedModes[0]);

        Assert.NotEqual(110, scaledPercentage);
        Assert.True(scaledPercentage < 110);
    }
}