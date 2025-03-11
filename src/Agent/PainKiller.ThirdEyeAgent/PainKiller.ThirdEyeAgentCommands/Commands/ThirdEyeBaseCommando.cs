using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.Data;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
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
    protected void ProjectSearch(ThirdPartyComponent component, bool detailedSearch)
    {
        var devProjects = Storage.GetDevProjects().Where(dp => dp.Components.Any(c => c.Name == component.Name && (c.Version == component.Version || !detailedSearch))).ToList();
        var projects = Storage.GetProjects().Where(p => devProjects.Any(dp => dp.ProjectId == p.Id)).ToList();
        var repos = new List<Repository>();
        var teams = Storage.GetTeams();
        foreach (var projectRepos in projects.Select(project => Storage.GetRepositories().Where(r => r.ProjectId == project.Id))) repos.AddRange(projectRepos);
        PresentationManager.DisplayOrganization(Configuration.ThirdEyeAgent.OrganizationName, projects, repos, teams, devProjects);
    }
}