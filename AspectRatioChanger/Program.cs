using AspectRatioChanger;
using Spectre.Console;

AnsiConsole.WriteLine("Change docked mode aspect ratios");

var fruit = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("What do you want to do?")
        .PageSize(3)
        .MoreChoicesText("[grey](Move up and down)[/]")
        .AddChoices(new[] {
            "List cores", "Reset docked modes", "Change docked modes",
        }));

// Echo the fruit back to the terminal
AnsiConsole.WriteLine($"I agree. {fruit} is tasty!");

Console.WriteLine("What drive do you want to look in?");
var drive = Console.ReadKey().Key;

var arc = new RatioHandler(drive.ToString());

arc.DoWork();

Console.WriteLine("Would you like to change Aspect Ratio for Docked mode?");
var yesNo =Console.ReadKey().Key;

if (yesNo == ConsoleKey.Y)
{
    arc.AddDockedModes();
    Console.WriteLine("Changed aspect ratios");
}

Console.ReadLine();
