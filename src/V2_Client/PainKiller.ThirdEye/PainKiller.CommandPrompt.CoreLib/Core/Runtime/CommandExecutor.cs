using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;

namespace PainKiller.CommandPrompt.CoreLib.Core.Runtime;

public class CommandExecutor
{ 
    private readonly ILogger<CommandExecutor> _logger = LoggerProvider.CreateLogger<CommandExecutor>();
    public RunResult Execute(IConsoleCommand? command, ICommandLineInput commandLineInput)
    {
        if (command == null)
        {
            _logger.LogWarning($"Command {command} not found");
            return new RunResult("",false,  "Command not found");
        }
        try
        {
            var runResult = command.Run(commandLineInput);
            return runResult;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);
            return new RunResult(command.Identifier, false, ex.Message);
        }
    }
}