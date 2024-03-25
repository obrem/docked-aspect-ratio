using AspectRatioChanger.Pocos;
using Spectre.Console;
using System.Text.Json;

namespace AspectRatioChanger;

public class RatioHandler
{
    readonly List<CoreDescription> cores = new();

    readonly string rootPath;
    private JsonSerializerOptions _jsonSerializerOptions;

    public RatioHandler(string drive)
    {
        rootPath = drive + @":\Cores";
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            WriteIndented = true
        };
    }
    public void DoWork()
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

    private void WriteToFile(string folderPath, bool reset)
    {

        var increasRate = 1.09;
        try
        {
            foreach (var file in Directory.GetFiles(folderPath, "video.json"))
            {

                string jsonContent = File.ReadAllText(file);
                var videoSettings = JsonSerializer.Deserialize<Root>(jsonContent, _jsonSerializerOptions);

                foreach (var mode in videoSettings.video.scaler_modes)
                {
                    if (reset)
                    {
                        mode.dock_aspect_w = null;
                        mode.dock_aspect_h = null;
                        continue;
                    }
                    if (mode.rotation == 90 || mode.rotation == 270)
                    {
                        mode.dock_aspect_h = (int)(mode.aspect_h * 100 * increasRate);
                        mode.dock_aspect_w = mode.aspect_w * 100;
                    }
                    else
                    {
                        mode.dock_aspect_w = (int)(mode.aspect_w * 100 * increasRate);
                        mode.dock_aspect_h = mode.aspect_h * 100;
                    }
                }
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
