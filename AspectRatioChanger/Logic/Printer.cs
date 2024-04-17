namespace AspectRatioChanger.Logic;

public static class Printer
{
    public static void PrintTitle()
    {
        AnsiConsole.Write(
            new FigletText("Docked AR Stretch")
                .LeftJustified()
                .Color(Color.Teal));
    }

    public static string PromptSelections()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What do you want to do?")
                .PageSize(4)
                .MoreChoicesText("[grey](Move up and down)[/]")
                .AddChoices(Constants.List, Constants.ListDetails, Constants.ChangeDocked, Constants.Reset, Constants.Quit));
    }

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
            var color = GetColor(core!.DockedPercentageAspectRatio);
            table.AddRow(
                new Markup(core.CoreName),
                new Markup(core.DockedPercentageAspectRatio + "%", color));
        }

        // Render the table to the console
        AnsiConsole.Write(table);
    }

    public static int GetStretchPercentage()
    {
        var stretchPercentage = AnsiConsole.Prompt(
            new TextPrompt<int>("With how many percent do you want to stretch the display?")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]That's not a valid percentage[/]")
                .Validate(age =>
                {
                    return age switch
                    {
                        <= 0 => ValidationResult.Error("[red]Must be at least 1%[/]"),
                        >= 60 => ValidationResult.Error("[red]Larger than 60% wont have any effect[/]"),
                        _ => ValidationResult.Success()
                    };
                }));
        return stretchPercentage;
    }

    private static Color GetColor(int percentage)
    {
        Color color;

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