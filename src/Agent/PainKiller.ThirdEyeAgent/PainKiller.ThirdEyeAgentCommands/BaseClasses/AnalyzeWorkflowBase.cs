using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Extensions;
using PainKiller.ThirdEyeAgentCommands.Managers;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.BaseClasses;

public class AnalyzeWorkflowBase(IConsoleWriter writer, PowerCommandsConfiguration configuration)
{
    protected readonly PresentationManager PresentationManager = new PresentationManager(writer);
    protected readonly List<Repository> AnalyzedRepositories = [];
    protected Team? Team;
    protected Workspace? Workspace;
    protected Repository? Repository;
    protected List<ComponentCve> VulnerableComponents = [];
    protected CveEntry? CveEntry;
    public virtual void Run(params string[] args)
    {

    }
    public virtual void Load()
    {
        ConsoleService.Service.Clear();
        writer.WriteHeadLine("Loading CVEs...");
        CveStorageService.Service.ReLoad();
    }
    public virtual Team? SelectTeam()
    {
        var teams = FilterService.Service.GetTeams(configuration.ThirdEyeAgent.Teams.ToList()).ToList();
        var selectedTeams = ListService.ListDialog("Chose Team", teams.Select(t => t.Name).ToList(), autoSelectIfOnlyOneItem: false);
        Team =  selectedTeams.Count <= 0 ? null : teams[selectedTeams.First().Key];
        return Team;
    }
    public virtual Workspace? SelectWorkspace(Team selectedTeam)
    {
        var workspaces = FilterService.Service.GetWorkspaces(configuration.ThirdEyeAgent.Workspaces.ToList(), selectedTeam).ToList();
        var selectedWorkspaces = ListService.ListDialog("Chose Workspace", workspaces.Select(w => w.Name).ToList());
        Workspace =  selectedWorkspaces.Count <= 0 ? null : workspaces[selectedWorkspaces.First().Key];
        return Workspace;
    }
    public virtual Repository? SelectRepository(Guid workspaceId)
    {
        var repositories = FilterService.Service.GetRepositories(workspaceId).ToList();
        var selectedRepositories = ListService.ListDialog("Chose Repository", repositories.Select(r => $"{r.Name} {AnalyzedRepositories.Any(re => re.RepositoryId == r.RepositoryId).ToCheck()}").ToList());
        if (selectedRepositories.Count <= 0) return null;
        var selectedRepository = repositories[selectedRepositories.First().Key];
        AnalyzedRepositories.Add(selectedRepository);
        Repository = selectedRepository;
        return Repository;
    }
    public virtual void UpdateInfoPanel()
    {
        ConsoleService.Service.Clear();
        writer.WriteLine("");
        IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
    }
    public virtual List<ComponentCve> GetVulnerableComponents(Repository? repository, string name = "")
    {
        var filteredThirdPartyComponents = repository == null ? ObjectStorageService.Service.GetThirdPartyComponents() : FilterService.Service.GetThirdPartyComponents(repository).ToList();
        if (!string.IsNullOrEmpty(name)) filteredThirdPartyComponents = filteredThirdPartyComponents.Where(c => c.Name.ToLower().Contains(name.ToLower())).ToList();
        var analyzer = new CveAnalyzeManager(writer);
        var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
        var components = analyzer.GetVulnerabilities(CveStorageService.Service.GetCveEntries(), filteredThirdPartyComponents, threshold);
        VulnerableComponents = PresentationManager.DisplayVulnerableComponents(components);
        return VulnerableComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder).ToList();
    }
    public virtual ComponentCve? ViewCveDetails(List<ComponentCve> componentCves)
    {
        var selected = ListService.ListDialog("Choose a component to view details.", componentCves.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
        if (selected.Count <= 0) return null;
        var component = componentCves[selected.First().Key];
        CveEntry = PresentationManager.DisplayVulnerableComponent(component);
        
        if (CveEntry == null) return null;
        
        var apiKey = configuration. Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
        var cveFetcher = new CveFetcherManager(CveStorageService.Service, configuration.ThirdEyeAgent.Nvd, apiKey, writer);
        var cve = cveFetcher.FetchCveDetailsAsync(CveEntry.Id).Result;
        if (cve == null) return null;
        PresentationManager.DisplayCveDetails(cve);
        return component;
    }
    public virtual void WorkspaceSearch(ThirdPartyComponent component, bool detailedSearch)
    {
        var projects = ObjectStorageService.Service.GetProjects().Where(dp => dp.Components.Any(c => c.Name == component.Name && (c.Version == component.Version || !detailedSearch))).ToList();
        var workspaces = ObjectStorageService.Service.GetWorkspaces().Where(p => projects.Any(dp => dp.WorkspaceId == p.Id)).ToList();
        var repos = new List<Repository>();
        var teams = ObjectStorageService.Service.GetTeams();
        foreach (var projectRepos in workspaces.Select(project => ObjectStorageService.Service.GetRepositories().Where(r => r.WorkspaceId == project.Id)))
        {
            foreach (var projectRepo in projectRepos)
            {
                if(projects.Any(p => p.RepositoryId == projectRepo.RepositoryId))
                    repos.Add(projectRepo);
            }
        }
        PresentationManager.DisplayOrganization(configuration.ThirdEyeAgent.OrganizationName, workspaces, repos, teams, projects, component, skipEmpty: true);
    }
    public virtual void SaveFinding(ComponentCve cve, CveEntry cveEntry)
    {
        writer.WriteCodeExample(cveEntry.Id, $"{cve.Name} {cve.Version} CVEs: {cve.CveEntries.Count}" );
        var storeFinding = DialogService.YesNoDialog("Do you want to safe this finding to work with it later?");
        if(!storeFinding) return;

        var affectedVersion = cve.VersionOrder;
        var versionCorrect = DialogService.YesNoDialog($"Is the affected version {cve.Version} of component {cve.Name} correct?");
        if (!versionCorrect) affectedVersion = DialogService.QuestionAnswerDialog("Input the correct version:").ToVersionOrder();
        var isLowerVersionsAffected = DialogService.YesNoDialog("Is lover version also affected?");
        var description = DialogService.QuestionAnswerDialog("Describe your finding");
        var projects = ObjectStorageService.Service.GetProjects().Where(p => p.Components.Any(c => c.Name == cve.Name && (cve.VersionOrder == affectedVersion || (cve.VersionOrder < affectedVersion && isLowerVersionsAffected)))).ToList();
        var finding = new Finding { AffectedProjects = projects, Created = DateTime.Now, Cve = cveEntry, Description = description, Status = FindingStatus.New, Updated = DateTime.Now };
        ObjectStorageService.Service.InsertFinding(finding);
    }
}