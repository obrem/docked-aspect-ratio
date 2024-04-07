using AspectRatioChanger.Pocos;
using Spectre.Console;
using System.Text.Json;

namespace AspectRatioChanger;

public class IOHandler
{
    readonly List<CoreDescription> cores = new();

    readonly string rootPath;
    private JsonSerializerOptions _jsonSerializerOptions;

    public IOHandler(string drive)
    {
        rootPath = drive + @":\Cores";
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            WriteIndented = true
        };
    }
    public void ListCores()
    {
        FindVideoJsonFiles(rootPath);
        Print(cores);
    }

    private void FindVideoJsonFiles(string folderPath)
    {
        try
        {
            foreach (var file in Directory.GetFiles(folderPath, "video.json"))
            {

                string jsonContent = File.ReadAllText(file);
                var videoSettings = JsonSerializer.Deserialize<Root>(jsonContent, _jsonSerializerOptions);

                foreach (var mode in videoSettings.video.scaler_modes)
                {
                    var core = new CoreDescription
                    {
                        CoreName = Path.GetDirectoryName(file).Split("\\").Last(),
                        Flipped = mode.rotation == 90 || mode.rotation == 270,
                        CurrentAspectRatio = mode.aspect_w + ":" + mode.aspect_h,
                        DockedAspectRatio = mode.dock_aspect_w + ":" + mode.dock_aspect_h
                    };

                    cores.Add(core);
                }
            }

            foreach (string directory in Directory.GetDirectories(folderPath))
            {
                FindVideoJsonFiles(directory); // Recursive call to search subdirectories
            }
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void Print(List<CoreDescription> cores)
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
            table.AddRow(core.CoreName, core.Flipped == false ? string.Empty : "Yes", core.CurrentAspectRatio,
               core.DockedAspectRatio);
        }
        // Render the table to the console
        AnsiConsole.Write(table);
    }

    public void AddDockedModes()
    {
        WriteToFile(rootPath, false);
    }

    public void ResetDockedModes()
    {
        WriteToFile(rootPath, true);
    }

    private void WriteToFile(string folderPath, bool reset)
    {

        var increaseRate = 1.1;
        try
        {
            foreach (var file in Directory.GetFiles(folderPath, "video.json"))
            {

                string jsonContent = File.ReadAllText(file);
                var videoSettings = JsonSerializer.Deserialize<Root>(jsonContent, _jsonSerializerOptions);

                var ratioHandler = new RatioHandler();
                var modifiedScalerModes = ratioHandler.AddDockedModes(videoSettings.video.scaler_modes, increaseRate, reset);
                videoSettings.video.scaler_modes = modifiedScalerModes;
                
                var stringJson = JsonSerializer.Serialize<Root>(videoSettings, _jsonSerializerOptions);
                File.WriteAllText(file, stringJson);
            }

            foreach (string directory in Directory.GetDirectories(folderPath))
            {
                WriteToFile(directory, reset); // Recursive call to search subdirectories
            }
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
