using AspectRatioChanger;
using Spectre.Console;

var title = @"    ___           _            _     _      __   __ _            _       _     
   /   \___   ___| | _____  __| |   /_\    /__\ / _\ |_ _ __ ___| |_ ___| |__  
  / /\ / _ \ / __| |/ / _ \/ _` |  //_\\  / \// \ \| __| '__/ _ \ __/ __| '_ \ 
 / /_// (_) | (__|   <  __/ (_| | /  _  \/ _  \ _\ \ |_| | |  __/ || (__| | | |
/___,' \___/ \___|_|\_\___|\__,_| \_/ \_/\/ \_/ \__/\__|_|  \___|\__\___|_| |_|                                                                              
";

AnsiConsole.MarkupLine("[teal]"+ title + "[/]");

Console.WriteLine("What drive do you want to look in?");
var drive = Console.ReadKey().Key;

var arc = new IOHandler(drive.ToString());


var action = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("What do you want to do?")
        .PageSize(4)
        .MoreChoicesText("[grey](Move up and down)[/]")
        .AddChoices(new[] {
            "List cores", "Change docked modes", "Reset docked modes", "Quit",
        }));

AnsiConsole.WriteLine($"Ok, lets {action}");


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
        break;
    default:
        break;
}

Console.WriteLine("Press any key to quit");
Console.ReadLine();
