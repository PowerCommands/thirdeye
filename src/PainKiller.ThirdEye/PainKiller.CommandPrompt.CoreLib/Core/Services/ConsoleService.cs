using Spectre.Console;
namespace PainKiller.CommandPrompt.CoreLib.Core.Services;
public static class ConsoleService
{
    public static readonly IConsoleWriter Writer = SpectreConsoleWriter.Instance;
    public static void WriteCenteredText(string headline, string text)
    {
        var centeredText = new FigletText(text)
            .Centered()
            .Color(Color.DarkMagenta);

        var panel = new Panel(centeredText)
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1),
            Header = new PanelHeader($"[blue]{headline}[/]", Justify.Center)
        };
        AnsiConsole.Write(panel);
    }
}