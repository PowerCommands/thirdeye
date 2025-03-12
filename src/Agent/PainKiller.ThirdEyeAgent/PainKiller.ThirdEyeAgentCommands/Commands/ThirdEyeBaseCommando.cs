using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Managers;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Commands;

public abstract class ThirdEyeBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    protected ThirdEyeBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration)
    {
        var gitHub = configuration.ThirdEyeAgent.Host.Contains("github.com");
        var accessToken = Configuration.Secret.DecryptSecret(ConfigurationGlobals.GetAccessTokenName(gitHub));
        ObjectStorageService.Initialize(configuration.ThirdEyeAgent.Host);
        Storage = ObjectStorageService.Service;
        GitManager = gitHub ? new GitHubManager(configuration.ThirdEyeAgent.Host, accessToken, configuration.ThirdEyeAgent.OrganizationName, this) : new AdsManager(Configuration.ThirdEyeAgent.Host, accessToken, this);
        PresentationManager = new PresentationManager(this);
        CveStorageService.Initialize(configuration.ThirdEyeAgent.Nvd.PathToUpdates);
        CveStorage = CveStorageService.Service;
    }
    protected ICveStorageService CveStorage { get; init; } 
    protected IObjectStorageService Storage { get; }
    protected IGitManager GitManager { get; } 
    protected IFileAnalyzeManager AnalyzeManager { get; } = new FileAnalyzeManager();
    protected PresentationManager PresentationManager { get; }
    protected void ProjectSearch(ThirdPartyComponent component, bool detailedSearch)
    {
        var projects = Storage.GetProjects().Where(dp => dp.Components.Any(c => c.Name == component.Name && (c.Version == component.Version || !detailedSearch))).ToList();
        var workspaces = Storage.GetWorkspaces().Where(p => projects.Any(dp => dp.WorkspaceId == p.Id)).ToList();
        var repos = new List<Repository>();
        var teams = Storage.GetTeams();
        foreach (var projectRepos in workspaces.Select(project => Storage.GetRepositories().Where(r => r.WorkspaceId == project.Id))) repos.AddRange(projectRepos);
        PresentationManager.DisplayOrganization(Configuration.ThirdEyeAgent.OrganizationName, workspaces, repos, teams, projects);
    }
}