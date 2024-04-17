using AspectRatioChanger;
using AspectRatioChanger.Logic;
using Spectre.Console;

Printer.PrintTitle();

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

do
{
    var action = Printer.PromptSelections();

    switch (action)
    {
        case Constants.List:
            arc.ListCores();
            break;
        case Constants.ListDetails:
            arc.ListCores(true);
            break;
        case Constants.ChangeDocked:
            arc.AddDockedModes();
            break;
        case Constants.Reset:
            arc.ResetDockedModes();
            break;
        case Constants.Quit:
            run = false;
            break;
        default:
            run = false;
            break;
    }
} while (run);



