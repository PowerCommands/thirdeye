using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.Data;
using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands;

public abstract class ThirdEyeBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    protected ThirdEyeBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration)
    {
        var gitHub = configuration.ThirdEyeAgent.Host.Contains("github.com");
        var accessToken = Configuration.Secret.DecryptSecret(ConfigurationGlobals.GetAccessTokenName(gitHub));
        Storage = new ObjectStorageManager(configuration.ThirdEyeAgent.Host);
        GitManager = gitHub ? new GitHubManager(configuration.ThirdEyeAgent.Host, accessToken, configuration.ThirdEyeAgent.OrganizationName, this) : new AdsManager(Configuration.ThirdEyeAgent.Host, accessToken, this);
        PresentationManager = new PresentationManager(this);
        CveStorageService.Initialize(configuration.ThirdEyeAgent.Nvd.PathToUpdates);
        CveStorage = CveStorageService.Service;
    }
    protected ICveStorageService CveStorage { get; init; } 
    protected IObjectStorageManager Storage { get; }
    protected IGitManager GitManager { get; } 
    protected IFileAnalyzeManager AnalyzeManager { get; } = new FileAnalyzeManager();
    protected PresentationManager PresentationManager { get; }
}