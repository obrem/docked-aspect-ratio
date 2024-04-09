using System.Text.Json;
using AspectRatioChanger.Pocos;
using Spectre.Console;

namespace AspectRatioChanger.Handlers;

public class IoHandler(string rootPath)
{
    private List<CoreDescription> _cores;
    private const bool ListDebugMode = true;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        TypeInfoResolver = SourceGenerationContext.Default,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true
    };

    public static string FindRootFolder()
    {
        // Look current folder to see if it to use that one
        var currentDir = Directory.GetCurrentDirectory();
        var folders = Directory.EnumerateDirectories(currentDir);
        var hasCoresFolder = folders.SingleOrDefault(f => f == currentDir + "/Cores");
        if (hasCoresFolder != null)
        {
            return currentDir;
        }


        // Search all mounted drives
        var drives = DriveInfo.GetDrives();
        foreach (var driveInfo in drives)
        {
            // Warning only selects the first one
            folders = Directory.EnumerateDirectories(driveInfo.Name);
            hasCoresFolder = folders.SingleOrDefault(f => f == driveInfo.Name + "Cores");
            if (hasCoresFolder != null)
            {
                return driveInfo.Name + "Cores";
            }
        }

        // Else ask for drive path
        AnsiConsole.WriteLine("Could not find AnaloguePocket Cores folder");
        var drive = AnsiConsole.Ask<string>("Type the path where your AnaloguePocket SD card is?");
        folders = Directory.EnumerateDirectories(drive);
        hasCoresFolder = folders.SingleOrDefault(f => f == "Cores");
        if (hasCoresFolder != null)
        {
            return drive + "/Cores";
        }
        return drive;
    }

    public void ListCores()
    {
        _cores = new List<CoreDescription>();
        FindVideoJsonFiles(rootPath);
        if (ListDebugMode)
        {
            if (_cores.Count != 0)
            {
                Printer.PrintDebug(_cores);
            }
        }
        else
        {
            Printer.Print(_cores);
        }
    }

    public void AddDockedModes()
    {
        var stretchPercentage = Printer.GetStretchPercentage();
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
                        var ratioHandler = new RatioHandler();
                        var core = new CoreDescription
                        {
                            CoreName = Path.GetDirectoryName(file).Split("\\").Last(),
                            Flipped = mode.rotation == 90 || mode.rotation == 270,
                            CurrentAspectRatio = mode.aspect_w + ":" + mode.aspect_h,
                            DockedAspectRatio = mode.dock_aspect_w + ":" + mode.dock_aspect_h,
                            DockedPercentageAspectRatio = ratioHandler.GetScaledPercentage(mode)
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

    private void WriteToFile(string folderPath, int increasePercentage, bool reset)
    {
        if (increasePercentage <= 0 && !reset)
            throw new ArgumentOutOfRangeException("increasePercentage", "Percentage can't be 0 or lower");

        var increaseRate = 1 + ((double)increasePercentage / 100);
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