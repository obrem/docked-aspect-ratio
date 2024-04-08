using System.Text.Json;
using AspectRatioChanger.Pocos;
using Spectre.Console;

namespace AspectRatioChanger.Handlers;

public class IoHandler(string rootPath)
{
    private readonly List<CoreDescription> _cores = new();
    private const bool ListDebugMode = false;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        TypeInfoResolver = SourceGenerationContext.Default,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true
    };



    public void ListCores()
    {
        FindVideoJsonFiles(rootPath);
        if (ListDebugMode)
        {
            if (_cores.Count != 0)
            {
                PrintDebug(_cores);
            }
        }
        else
        {
            Print(_cores);
        }
    }

    public void AddDockedModes()
    {
        var stretchPercentage = AnsiConsole.Prompt(
            new TextPrompt<int>("With how many percent do you want to stretch the display?")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]That's not a valid percentage[/]")
                .Validate(age =>
                {
                    return age switch
                    {
                        <= 0 => ValidationResult.Error("[red]Must be at least 1%[/]"),
                        >= 60 => ValidationResult.Error("[red]Larger than 60% wont have any effect[/]"),
                        _ => ValidationResult.Success()
                    };
                }));

        WriteToFile(rootPath, stretchPercentage, false);
    }

    public void ResetDockedModes()
    {
        WriteToFile(rootPath, 0, true);
    }

    private void FindVideoJsonFiles(string folderPath)
    {
        try
        {
            foreach (var file in Directory.GetFiles(folderPath, "video.json"))
            {
                var jsonContent = File.ReadAllText(file);
                var videoSettings = JsonSerializer.Deserialize(jsonContent, typeof(Root), _jsonSerializerOptions) as Root;

                if (videoSettings != null)
                    foreach (var mode in videoSettings.video.scaler_modes)
                    {
                        var scalingPercentage = 100;

                        if (mode.dock_aspect_w != null)
                        {
                            var normalAr = (decimal)mode.aspect_w / (decimal)mode.aspect_h;
                            var dockedAr = (decimal)mode.dock_aspect_w / (decimal)mode.dock_aspect_h;
                            scalingPercentage = (int)(Math.Round(((dockedAr + normalAr) / 2) / (dockedAr - normalAr), MidpointRounding.AwayFromZero) + 100);
                        }

                        var core = new CoreDescription
                        {
                            CoreName = Path.GetDirectoryName(file).Split("\\").Last(),
                            Flipped = mode.rotation == 90 || mode.rotation == 270,
                            CurrentAspectRatio = mode.aspect_w + ":" + mode.aspect_h,
                            DockedAspectRatio = mode.dock_aspect_w + ":" + mode.dock_aspect_h,
                            DockedPercentageAspectRatio = scalingPercentage
                        };

                        _cores.Add(core);
                    }
            }

            foreach (var directory in Directory.GetDirectories(folderPath))
            {
                FindVideoJsonFiles(directory); // Recursive call to search subdirectories
            }
        }
        catch (UnauthorizedAccessException e)
        {
            AnsiConsole.WriteLine(e.Message);
        }
        catch (DirectoryNotFoundException)
        {
            AnsiConsole.WriteLine("No cores found under given directory");
        }
    }

    private static void PrintDebug(List<CoreDescription> cores)
    {
        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn("Name");
        table.AddColumn("Flipped");
        table.AddColumn("AspectRatio");
        table.AddColumn("Docked AR");

        // Add some rows
        foreach (var core in cores)
        {
            table.AddRow(core.CoreName, core.Flipped ? "Yes" : string.Empty, core.CurrentAspectRatio, core.DockedAspectRatio ?? string.Empty);
        }

        // Render the table to the console
        AnsiConsole.Write(table);
    }


    private static void Print(List<CoreDescription> cores)
    {
        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn("Name");
        table.AddColumn("Docked AR");

        var grouped = cores.GroupBy(core => core.CoreName).Select(g => g.MinBy(x => x.CoreName));
        foreach (var core in grouped)
        {
            //var percentage = ( - b) / (a + b) / 2 | × 100 %
            table.AddRow(core.CoreName, core.DockedPercentageAspectRatio +"%" );
        }

        // Render the table to the console
        AnsiConsole.Write(table);
    }

    private void WriteToFile(string folderPath, int increasePercentage, bool reset)
    {
        if (increasePercentage <= 0)
            throw new ArgumentOutOfRangeException("increasePercentage", "Percentage can't be 0 or lower");

        var increaseRate = 1 + (double)increasePercentage / 10;
        try
        {
            foreach (var file in Directory.GetFiles(folderPath, "video.json"))
            {
                var jsonContent = File.ReadAllText(file);
                var videoSettings = JsonSerializer.Deserialize(jsonContent, typeof(Root), _jsonSerializerOptions) as Root;

                var ratioHandler = new RatioHandler();
                var modifiedScalerModes = ratioHandler.AddDockedModes(videoSettings.video.scaler_modes, increaseRate, reset);
                videoSettings.video.scaler_modes = modifiedScalerModes;

                var stringJson = JsonSerializer.Serialize(videoSettings, typeof(Root), _jsonSerializerOptions);
                File.WriteAllText(file, stringJson);
            }

            foreach (var directory in Directory.GetDirectories(folderPath))
            {
                WriteToFile(directory, increasePercentage, reset); // Recursive call to search subdirectories
            }
        }
        catch (UnauthorizedAccessException e)
        {
            AnsiConsole.WriteLine(e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            AnsiConsole.WriteLine(e.Message);
        }
    }
}