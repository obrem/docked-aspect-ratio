using System.Text.Json;
using AspectRatioChanger.Pocos;
using Spectre.Console;

namespace AspectRatioChanger.Handlers;

public class IoHandler(string drive)
{
    private readonly List<CoreDescription> _cores = new();

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        TypeInfoResolver = SourceGenerationContext.Default,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true
    };

    private readonly string _rootPath = drive + @":\Cores";


    public void ListCores()
    {
        FindVideoJsonFiles(_rootPath);
        if (_cores.Count != 0)
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

        WriteToFile(_rootPath, stretchPercentage, false);
    }

    public void ResetDockedModes()
    {
        WriteToFile(_rootPath, 0, true);
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
                        var core = new CoreDescription
                        {
                            CoreName = Path.GetDirectoryName(file).Split("\\").Last(),
                            Flipped = mode.rotation == 90 || mode.rotation == 270,
                            CurrentAspectRatio = mode.aspect_w + ":" + mode.aspect_h,
                            DockedAspectRatio = mode.dock_aspect_w + ":" + mode.dock_aspect_h
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

    private static void Print(List<CoreDescription> cores)
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

    private void WriteToFile(string folderPath, int increasePercentage, bool reset)
    {
        if (increasePercentage <= 0)
            throw new ArgumentOutOfRangeException("increasePercentage can't be 0 or lower");

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