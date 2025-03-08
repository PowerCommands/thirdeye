using Microsoft.Extensions.Logging;
using PainKiller.PowerCommands.Shared.Contracts;

namespace PainKiller.PowerCommands.Shared.Utils;

/// <summary>
/// Does not write anything to the console, just writes to the log file.
/// </summary>
public class QuietConsoleWriter(ILogger log) : IConsoleWriter
{
    public void WriteLine(string output) => log.LogInformation($"{output}");
    public void WriteHeadLine(string output) => log.LogInformation($"{output}");
    public void Write(string output, ConsoleColor? color = null) => log.LogInformation($"{output}");
    public void WriteSuccess(string output) => log.LogInformation($"{output}");
    public void WriteSuccessLine(string output) => log.LogInformation($"{output}");
    public void WriteFailure(string output) => log.LogInformation($"{output}");
    public void WriteFailureLine(string output) => log.LogInformation($"{output}");
    public void WriteCodeExample(string commandName, string text) => log.LogInformation($"{commandName} {text}");
}