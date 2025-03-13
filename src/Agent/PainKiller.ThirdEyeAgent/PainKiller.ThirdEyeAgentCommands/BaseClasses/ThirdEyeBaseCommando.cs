using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Managers;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.BaseClasses;

public abstract class ThirdEyeBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    private readonly List<Repository> _analyzedRepositories = [];
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
        PresentationManager.DisplayOrganization(Configuration.ThirdEyeAgent.OrganizationName, workspaces, repos, teams, projects, skipEmpty: true);
    }

    protected void Analyse(Workspace selectedWorkspace)
    {
        var repositories = FilterService.Service.GetRepositories(selectedWorkspace.Id).ToList();
        var selectedRepositories = ListService.ListDialog("Chose Repository", repositories.Select(r => $"{r.Name} {_analyzedRepositories.Any(re => re.RepositoryId == r.RepositoryId).ToCheck()}").ToList());
        if (selectedRepositories.Count <= 0) return;
        var selectedRepository = repositories[selectedRepositories.First().Key];
        _analyzedRepositories.Add(selectedRepository);

        var filteredThirdPartyComponents = FilterService.Service.GetThirdPartyComponents(selectedRepository).ToList();

        ConsoleService.Service.Clear();
        WriteLine("");
        IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();

        var analyzer = new CveAnalyzeManager(this);
        var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();

        var components = analyzer.GetVulnerabilities(CveStorage.GetCveEntries(), filteredThirdPartyComponents, threshold);
        var selectedComponentCves = PresentationManager.DisplayVulnerableComponents(components);
        var selected = ListService.ListDialog("Choose a component to view details.", selectedComponentCves.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
        if (selected.Count <= 0) return;
        var component = selectedComponentCves[selected.First().Key];
        var componentCve = PresentationManager.DisplayVulnerableComponent(component);
        if (componentCve != null)
        {
            var apiKey = Configuration.Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
            var cveFetcher = new CveFetcherManager(CveStorage, Configuration.ThirdEyeAgent.Nvd, apiKey, this);
            var cve = cveFetcher.FetchCveDetailsAsync(componentCve.Id).Result;
            if (cve != null)
            {
                PresentationManager.DisplayCveDetails(cve);
            }
        }
        WriteLine("");
        var thirdPartyComponent = Storage.GetThirdPartyComponents().First(c => c.Name == component.Name && c.Version == component.Version);
        ProjectSearch(thirdPartyComponent, detailedSearch: true);
    }
}