using PainKiller.PowerCommands.Core.Commands;
using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands;

public abstract class ThirdEyeBaseCommando : CdCommand
{
    protected ThirdEyeBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration)
    {
        var gitHub = configuration.ThirdEyeAgent.Host.Contains("github.com");
        var accessToken = Configuration.Secret.DecryptSecret(ConfigurationGlobals.GetAccessTokenName(gitHub));
        ObjectStorage = new ObjectStorageManager(configuration.ThirdEyeAgent.Host);
        GitManager = gitHub ? new GitHubManager(configuration.ThirdEyeAgent.Host, accessToken, configuration.ThirdEyeAgent.OrganizationName, this) : new AdsManager(configuration.ThirdEyeAgent.Host, accessToken, this);;
        PresentationManager = new PresentationManager(this);
        Configuration = configuration;
    }
    protected IObjectStorageManager ObjectStorage { get; }
    protected IGitManager GitManager { get; } 
    protected IFileAnalyzeManager AnalyzeManager { get; } = new FileAnalyzeManager();
    protected PresentationManager PresentationManager { get; }

    public override void OnWorkingDirectoryChanged(string[] files, string[] directories)
    {
        base.OnWorkingDirectoryChanged(files, directories);
        if (IPowerCommandServices.DefaultInstance != null) IPowerCommandServices.DefaultInstance.InfoPanelManager.Display();
    }
}