using AspectRatioChanger.Logic;
using AspectRatioChanger.Pocos;
using Spectre.Console;
using System.Text.Json;

namespace AspectRatioChanger.Test.Logic;

public class IoHandlerTests : IDisposable
{
    private readonly string _originalCurrentDirectory = Directory.GetCurrentDirectory();
    private readonly string _temporaryRoot = Path.Combine(Path.GetTempPath(), "AspectRatioChangerTests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void FindRootFolder_CurrentDirectoryContainsCores_ReturnsCurrentDirectory()
    {
        // Arrange
        Directory.CreateDirectory(_temporaryRoot);
        Directory.CreateDirectory(Path.Combine(_temporaryRoot, "Cores"));
        Directory.SetCurrentDirectory(_temporaryRoot);

        // Act
        var result = IoHandler.FindRootFolder();

        // Assert
        Assert.Equal(Directory.GetCurrentDirectory(), result);
    }

    [Fact]
    public void ListCores_WhenVideoJsonExistsInNestedDirectory_WritesSummaryTable()
    {
        // Arrange
        var videoFilePath = CreateVideoJson("Core A", new VideoRoot { aspect_w = 4, aspect_h = 3, dock_aspect_w = 14, dock_aspect_h = 9 });
        var writer = new StringWriter();
        var originalConsole = AnsiConsole.Console;
        AnsiConsole.Console = CreateConsole(writer);

        try
        {
            var sut = new IoHandler(_temporaryRoot);

            // Act
            sut.ListCores();

            // Assert
            Assert.Contains("Core A", writer.ToString());
        }
        finally
        {
            AnsiConsole.Console = originalConsole;
        }
    }

    [Fact]
    public void ResetDockedModes_WhenVideoJsonExists_RemovesDockedAspectRatioValues()
    {
        // Arrange
        var videoFilePath = CreateVideoJson("Core B", new VideoRoot { aspect_w = 4, aspect_h = 3, dock_aspect_w = 14, dock_aspect_h = 9 });
        var sut = new IoHandler(_temporaryRoot);

        // Act
        sut.ResetDockedModes();

        // Assert
        var root = ReadVideoJson(videoFilePath);
        Assert.Null(root.video.scaler_modes[0].dock_aspect_w);
        Assert.Null(root.video.scaler_modes[0].dock_aspect_h);
    }

    [Fact]
    public void ResetDockedModes_WhenVideoJsonExistsInNestedDirectory_RemovesDockedAspectRatioValues()
    {
        // Arrange
        var videoFilePath = CreateVideoJson(Path.Combine("Parent", "Core C"), new VideoRoot { aspect_w = 4, aspect_h = 3, dock_aspect_w = 14, dock_aspect_h = 9 });
        var sut = new IoHandler(_temporaryRoot);

        // Act
        sut.ResetDockedModes();

        // Assert
        var root = ReadVideoJson(videoFilePath);
        Assert.Null(root.video.scaler_modes[0].dock_aspect_w);
        Assert.Null(root.video.scaler_modes[0].dock_aspect_h);
    }

    [Fact]
    public void ListCores_WhenDetailsRequested_WritesDetailsTable()
    {
        // Arrange
        CreateVideoJson("Core D", new VideoRoot { aspect_w = 8, aspect_h = 7, dock_aspect_w = 80, dock_aspect_h = 88, rotation = 90 });
        var writer = new StringWriter();
        var originalConsole = AnsiConsole.Console;
        AnsiConsole.Console = CreateConsole(writer);

        try
        {
            var sut = new IoHandler(_temporaryRoot);

            // Act
            sut.ListCores(true);

            // Assert
            Assert.Contains("Core D", writer.ToString());
        }
        finally
        {
            AnsiConsole.Console = originalConsole;
        }
    }

    private string CreateVideoJson(string relativeCorePath, VideoRoot scalerMode)
    {
        var coreDirectory = Path.Combine(_temporaryRoot, relativeCorePath);
        Directory.CreateDirectory(coreDirectory);
        var videoFilePath = Path.Combine(coreDirectory, "video.json");
        var root = new Root
        {
            video = new Video
            {
                magic = "APF_VER_1",
                scaler_modes = [scalerMode],
                display_modes = []
            }
        };

        File.WriteAllText(videoFilePath, JsonSerializer.Serialize(root));
        return videoFilePath;
    }

    private static Root ReadVideoJson(string videoFilePath)
    {
        return JsonSerializer.Deserialize<Root>(File.ReadAllText(videoFilePath))!;
    }

    private static IAnsiConsole CreateConsole(TextWriter writer, InteractionSupport interactionSupport = InteractionSupport.No)
    {
        return AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Interactive = interactionSupport,
            Out = new AnsiConsoleOutput(writer)
        });
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalCurrentDirectory);

        if (Directory.Exists(_temporaryRoot))
        {
            Directory.Delete(_temporaryRoot, true);
        }
    }
}
