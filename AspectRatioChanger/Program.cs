using AspectRatioChanger.Handlers;
using Spectre.Console;
using System.IO;

AnsiConsole.Write(
    new FigletText("Docked AR Stretch")
        .LeftJustified()
        .Color(Color.Teal));

var hasFolderPath = false;
var driveLocation = "D:/";
do
{

    driveLocation = FindRootFolder();
    if (AnsiConsole.Confirm($"Is this the path you want to use? '{driveLocation}'"))
    {
        hasFolderPath = true;
    }

} while (!hasFolderPath);


var arc = new IoHandler(driveLocation);
var run = true;

var options = new[] { "List cores", "Change docked modes", "Reset docked modes", "Quit" };

do
{
    var action = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What do you want to do?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down)[/]")
            .AddChoices(options));

    switch (action)
    {
        case "List cores":
            arc.ListCores();
            break;
        case "Change docked modes":
            arc.AddDockedModes();
            break;
        case "Reset docked modes":
            arc.ResetDockedModes();
            break;
        case "Quit":
            run = false;
            break;
        default:
            run = false;
            break;
    }
} while (run);

string FindRootFolder()
{
    // Look current folder to see if it to use that one
    var currentDir = Directory.GetCurrentDirectory();
    var folders = Directory.EnumerateDirectories(currentDir);
    var hasCoresFolder = folders.SingleOrDefault(f => f == "Cores");
    if (hasCoresFolder != null)
    {
        return currentDir;
    }


    var drives = DriveInfo.GetDrives();
    foreach (var driveInfo in drives)
    {
        // Warning only selects the first one
        folders = Directory.EnumerateDirectories(driveInfo.Name);
        hasCoresFolder = folders.SingleOrDefault(f => f == "Cores");
        if (hasCoresFolder != null)
        {
            return currentDir;
        }
    }

    // Else ask for drive path
    AnsiConsole.WriteLine("Could not find AnaloguePocket Cores folder");
    var drive = AnsiConsole.Ask<string>("Type the path where your AnaloguePocket SD card is?");
    folders = Directory.EnumerateDirectories(drive);
    hasCoresFolder = folders.SingleOrDefault(f => f == "Cores");
    if (hasCoresFolder != null)
    {
        return currentDir;
    }
    return drive;
}