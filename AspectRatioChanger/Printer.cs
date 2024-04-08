using AspectRatioChanger.Pocos;
using Spectre.Console;
using Color = Spectre.Console.Color;

namespace AspectRatioChanger;

public class Printer
{
    public static void PrintDebug(List<CoreDescription> cores)
    {
        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn("Name");
        table.AddColumn("Flipped");
        table.AddColumn("AspectRatio");
        table.AddColumn("Docked AR");
        table.AddColumn("Docked AR %");

        // Add some rows
        foreach (var core in cores)
        {

            var color = GetColor(core.DockedPercentageAspectRatio);

            table.AddRow(new Markup(core.CoreName), 
                new Markup(core.Flipped ? "Yes" : string.Empty), 
                new Markup(core.CurrentAspectRatio),
                new Markup(core.DockedAspectRatio ?? string.Empty), 
                new Markup($"{core.DockedPercentageAspectRatio}%", color));
        }

        // Render the table to the console
        AnsiConsole.Write(table);
    }

    public static void Print(List<CoreDescription> cores)
    {
        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn("Name");
        table.AddColumn("Docked AR");

        var grouped = cores.GroupBy(core => core.CoreName).Select(g => g.MinBy(x => x.CoreName));
        foreach (var core in grouped)
        {
            var color = GetColor(core.DockedPercentageAspectRatio);
            table.AddRow(
                new Markup(core.CoreName),
                new Markup(core.DockedPercentageAspectRatio + "%", color));
        }

        // Render the table to the console
        AnsiConsole.Write(table);
    }

    private static Spectre.Console.Color GetColor(int percentage)
    {
        var color = Color.Grey;

        if (percentage <= 107)
        {
            color = Color.Green;
        }
        else if (percentage <= 114)
        {
            color = Color.Yellow;
        }
        else if (percentage <= 121)
        {
            color = Color.DarkOrange;
        }
        else
        {
            color = Color.Red;
        }

        return color;
    }
}