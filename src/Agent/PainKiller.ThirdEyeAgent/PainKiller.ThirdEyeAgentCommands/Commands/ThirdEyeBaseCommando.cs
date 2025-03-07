using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands;

public abstract class ThirdEyeBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    protected ThirdEyeBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration)
    {
        ObjectStorage = new ObjectStorageManager(configuration.ThirdEyeAgent.Host);
        AdsManager = new AdsManager(Configuration.ThirdEyeAgent.Host,Configuration.Secret.DecryptSecret(ConfigurationGlobals.AccessTokenName), this);
        PresentationManager = new PresentationManager(this);
    }
    protected ObjectStorageManager ObjectStorage { get; }
    protected AdsManager AdsManager { get; } 
    protected FileAnalyzeManager AnalyzeManager { get; } = new();
    protected PresentationManager PresentationManager { get; }
}