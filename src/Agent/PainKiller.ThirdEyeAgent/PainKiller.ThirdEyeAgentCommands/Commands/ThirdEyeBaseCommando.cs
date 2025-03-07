using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands;

public abstract class ThirdEyeBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    protected ThirdEyeBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration)
    {
        var gitHub = configuration.ThirdEyeAgent.Host.Contains("github.com");
        var accessToken = Configuration.Secret.DecryptSecret(ConfigurationGlobals.GetAccessTokenName(gitHub));
        ObjectStorage = new ObjectStorageManager(configuration.ThirdEyeAgent.Host);
        GitManager = gitHub ? new GitHubManager(configuration.ThirdEyeAgent.Host, accessToken, configuration.ThirdEyeAgent.OrganisationName, this) : new AdsManager(Configuration.ThirdEyeAgent.Host, accessToken, this);;
        PresentationManager = new PresentationManager(this);
    }
    protected ObjectStorageManager ObjectStorage { get; }
    protected IGitManager GitManager { get; } 
    protected FileAnalyzeManager AnalyzeManager { get; } = new();
    protected PresentationManager PresentationManager { get; }
}