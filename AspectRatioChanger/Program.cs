using AspectRatioChanger;
using Spectre.Console;

var title = @"    ___           _            _     _      __   __ _            _       _     
   /   \___   ___| | _____  __| |   /_\    /__\ / _\ |_ _ __ ___| |_ ___| |__  
  / /\ / _ \ / __| |/ / _ \/ _` |  //_\\  / \// \ \| __| '__/ _ \ __/ __| '_ \ 
 / /_// (_) | (__|   <  __/ (_| | /  _  \/ _  \ _\ \ |_| | |  __/ || (__| | | |
/___,' \___/ \___|_|\_\___|\__,_| \_/ \_/\/ \_/ \__/\__|_|  \___|\__\___|_| |_|     

";

AnsiConsole.MarkupLine("[teal]" + title + "[/]");

var driveLocation = FindDrive();


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

string FindDrive()
{
    // Look current folder to see if it to use that one

    // Else search mounted drives for folders

    // Else ask for drive path
    var drive = AnsiConsole.Ask<string>("What drive do you want to look in?");
    return drive;
}