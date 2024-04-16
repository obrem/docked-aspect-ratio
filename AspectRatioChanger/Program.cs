using AspectRatioChanger.Handlers;
using Spectre.Console;

AnsiConsole.Write(
    new FigletText("Docked AR Stretch")
        .LeftJustified()
        .Color(Color.Teal));

var hasFolderPath = false;
string driveLocation;
var firstAttempt = true;
do
{
    driveLocation = IoHandler.FindRootFolder(firstAttempt);
    if (AnsiConsole.Confirm($"Is this the path you want to use? '{driveLocation}'"))
    {
        hasFolderPath = true;
    }
    firstAttempt = false;

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

