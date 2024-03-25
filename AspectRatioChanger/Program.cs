using AspectRatioChanger;

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
