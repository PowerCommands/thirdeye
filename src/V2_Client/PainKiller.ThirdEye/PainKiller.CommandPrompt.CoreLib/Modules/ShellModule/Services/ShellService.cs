using System.Diagnostics;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
public class ShellService : IShellService
{
    private static readonly Lazy<IShellService> Instance = new(() => new ShellService());
    private ShellService() { }
    public static IShellService Default => Instance.Value;
    public void OpenDirectory(string path)
    {
        var actualPath = ReplacePlaceholders(path);
        if (!Directory.Exists(actualPath)) return;

        Process.Start(new ProcessStartInfo
        {
            FileName = actualPath,
            UseShellExecute = true,
            Verb = "open"
        });
    }

    public void OpenWithDefaultProgram(string path, string workingDirectory = "")
    {
        var actualPath = ReplacePlaceholders(path);

        Process.Start(new ProcessStartInfo
        {
            FileName = actualPath,
            WorkingDirectory = ReplacePlaceholders(workingDirectory),
            UseShellExecute = true,
            Verb = "open"
        });
    }

    public void Execute(string program, string args = "", string workingDirectory = "", bool waitForExit = false)
    {
        var psi = new ProcessStartInfo
        {
            FileName = ReplacePlaceholders(program),
            Arguments = args,
            WorkingDirectory = ReplacePlaceholders(workingDirectory),
            RedirectStandardOutput = !waitForExit,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
        if (waitForExit)
        {
            process!.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
        }
    }
    private static string ReplacePlaceholders(string input)
    {
        return input
            .Replace("$ROAMING$", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
            .Replace("%USERNAME%", Environment.UserName, StringComparison.OrdinalIgnoreCase);
    }
}