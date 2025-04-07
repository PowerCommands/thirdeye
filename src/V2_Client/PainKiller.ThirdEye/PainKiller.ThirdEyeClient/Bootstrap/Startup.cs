using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Configuration.Services;
using PainKiller.CommandPrompt.CoreLib.Core.Events;
using PainKiller.CommandPrompt.CoreLib.Core.Runtime;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.ReadLine;
using PainKiller.ThirdEyeClient.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace PainKiller.ThirdEyeClient.Bootstrap;

public static class Startup
{
    public static CommandLoop Build()
    {
        var config = ReadConfiguration();
        
        var logger = LoggerProvider.CreateLogger<Program>();
        logger.LogInformation($"{config.Core.Name} started, configuration read and logging initialized.");

        if(!Directory.Exists(Path.Combine(ApplicationConfiguration.CoreApplicationDataPath, config.Core.RoamingDirectory))) Directory.CreateDirectory(Path.Combine(ApplicationConfiguration.CoreApplicationDataPath, config.Core.RoamingDirectory));

        Console.CursorTop = config.Core.Modules.InfoPanel.Height;
        ShowLogo(config.Core);
        EventBusService.Service.Subscribe<SetupRequiredEventArgs>(args =>
        {
            logger.LogInformation($"Setup required: {args.Description}");
            args.SetupAction?.Invoke();
        });
        var commands = CommandDiscoveryService.DiscoverCommands(config);
        foreach (var consoleCommand in commands) consoleCommand.OnInitialized();
        Console.CursorTop = config.Core.Modules.InfoPanel.Height + 10;

        var suggestions = new List<string>();
        suggestions.AddRange(config.Core.Suggestions);
        suggestions.AddRange(commands.Select(c => c.Identifier).ToArray());
        ReadLineService.InitializeAutoComplete([], suggestions.ToArray());
        
        logger.LogDebug($"Suggestions: {string.Join(',', suggestions)}");
        
        EventBusService.Service.Publish(new WorkingDirectoryChangedEventArgs(Environment.CurrentDirectory));
        logger.LogDebug($"{nameof(EventBusService)} publish: {nameof(WorkingDirectoryChangedEventArgs)} {Environment.CurrentDirectory}");

        return new CommandLoop(new CommandRuntime(commands), new ReadLineInputReader(), config.Core);
    }
    private static CommandPromptConfiguration ReadConfiguration()
    {
        var configuration = ConfigurationService.Service.Get<CommandPromptConfiguration>();
        ConfigureLogging(configuration.Configuration.Log);
        return configuration.Configuration;
    }
    private static void ConfigureLogging(LogConfiguration config)
    {
        var parsedLevel = Enum.TryParse<LogEventLevel>(config.RestrictedToMinimumLevel, ignoreCase: true, out var minimumLevel) ? minimumLevel : LogEventLevel.Information;
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(parsedLevel)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: Path.Combine(config.FilePath, config.FileName),
                rollingInterval: (RollingInterval) Enum.Parse(typeof(RollingInterval), config.RollingIntervall),
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {MachineName}/{EnvironmentUserName} {SourceContext}: {Message:lj}{NewLine}{Exception}"
            );
        var serilogLogger = loggerConfig.CreateLogger();
        var loggerFactory = new SerilogLoggerFactory(serilogLogger);
        LoggerProvider.Configure(loggerFactory);
    }
    internal static void ShowLogo(CoreConfiguration config)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        if(!config.ShowLogo) return;
        ConsoleService.WriteCenteredText($" Version {config.Version} ", $"{config.Name}");
        Console.WriteLine();
    }
}