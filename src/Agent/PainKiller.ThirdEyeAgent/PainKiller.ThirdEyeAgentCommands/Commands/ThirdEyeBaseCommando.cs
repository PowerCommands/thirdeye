using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands;

public abstract class ThirdEyeBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    protected ThirdEyeBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration)
    {
        DbManager = new DbManager(configuration.ThirdEyeAgent.Host);
        AdsManager = new AdsManager(Configuration.ThirdEyeAgent.Host,Configuration.Secret.DecryptSecret(ConfigurationGlobals.AccessTokenName), this);
    }
    protected DbManager DbManager { get; }
    protected AdsManager AdsManager { get; } 
    protected FileAnalyzeManager AnalyzeManager { get; } = new();
}