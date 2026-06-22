using AspectRatioChanger.Logic;
using AspectRatioChanger.Pocos;
using Moq;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace AspectRatioChanger.Test;

public class PrinterTests : IDisposable
{
    private readonly IAnsiConsole _originalConsole;

    public PrinterTests()
    {
        _originalConsole = AnsiConsole.Console;
    }

    public void Dispose()
    {
        AnsiConsole.Console = _originalConsole;
    }

    [Fact]
    public void PrintTitle_WhenCalled_WritesFigletTitle()
    {
        // Arrange
        var console = new Mock<IAnsiConsole>();
        AnsiConsole.Console = console.Object;

        // Act
        Printer.PrintTitle();

        // Assert
        console.Verify(x => x.Write(It.Is<IRenderable>(renderable => renderable is FigletText)), Times.Once);
    }

    [Fact(Skip = "Spectre.Console interactive prompts require real console key input in this test host.")]
    public void PromptSelections_WhenDefaultSelectionAccepted_ReturnsListSelection()
    {
        // Arrange
        var writer = new StringWriter();
        var settings = new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Interactive = InteractionSupport.No,
            Out = new AnsiConsoleOutput(writer)
        };
        AnsiConsole.Console = AnsiConsole.Create(settings);

        // Act
        var selection = Printer.PromptSelections();

        // Assert
        Assert.Equal(Constants.List, selection);
    }

    [Fact]
    public void PrintDebug_WhenCoresContainAllDisplayStates_WritesTable()
    {
        // Arrange
        var console = new Mock<IAnsiConsole>();
        AnsiConsole.Console = console.Object;
        var cores = new List<CoreDescription>
        {
            new()
            {
                CoreName = "Green",
                CurrentAspectRatio = "4:3",
                DockedAspectRatio = "14:9",
                DockedPercentageAspectRatio = 107,
                Flipped = true
            },
            new()
            {
                CoreName = "Yellow",
                CurrentAspectRatio = "8:7",
                DockedAspectRatio = null,
                DockedPercentageAspectRatio = 114,
                Flipped = false
            },
            new()
            {
                CoreName = "Orange",
                CurrentAspectRatio = "5:4",
                DockedAspectRatio = "16:10",
                DockedPercentageAspectRatio = 121,
                Flipped = false
            },
            new()
            {
                CoreName = "Red",
                CurrentAspectRatio = "1:1",
                DockedAspectRatio = "16:9",
                DockedPercentageAspectRatio = 122,
                Flipped = true
            }
        };

        // Act
        Printer.PrintDebug(cores);

        // Assert
        console.Verify(x => x.Write(It.Is<IRenderable>(renderable => renderable is Table)), Times.Once);
    }

    [Fact]
    public void Print_WhenCoresContainDuplicateNames_WritesGroupedTable()
    {
        // Arrange
        var console = new Mock<IAnsiConsole>();
        AnsiConsole.Console = console.Object;
        var cores = new List<CoreDescription>
        {
            new()
            {
                CoreName = "Core A",
                CurrentAspectRatio = "4:3",
                DockedAspectRatio = "14:9",
                DockedPercentageAspectRatio = 107,
                Flipped = false
            },
            new()
            {
                CoreName = "Core A",
                CurrentAspectRatio = "8:7",
                DockedAspectRatio = "16:10",
                DockedPercentageAspectRatio = 115,
                Flipped = true
            },
            new()
            {
                CoreName = "Core B",
                CurrentAspectRatio = "1:1",
                DockedAspectRatio = "16:9",
                DockedPercentageAspectRatio = 122,
                Flipped = false
            }
        };

        // Act
        Printer.Print(cores);

        // Assert
        console.Verify(x => x.Write(It.Is<IRenderable>(renderable => renderable is Table)), Times.Once);
    }

    [Fact(Skip = "Spectre.Console interactive prompts require real console key input in this test host.")]
    public void GetStretchPercentage_WhenInputIsValid_ReturnsPercentage()
    {
        // Arrange
        var reader = new StringReader("25" + Environment.NewLine);
        var writer = new StringWriter();
        var originalIn = Console.In;
        Console.SetIn(reader);
        var settings = new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Interactive = InteractionSupport.Yes,
            Out = new AnsiConsoleOutput(writer)
        };
        AnsiConsole.Console = AnsiConsole.Create(settings);

        try
        {
            // Act
            var percentage = Printer.GetStretchPercentage();

            // Assert
            Assert.Equal(25, percentage);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }
}
