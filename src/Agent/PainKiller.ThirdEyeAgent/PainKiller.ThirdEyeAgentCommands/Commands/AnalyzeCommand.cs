using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Managers;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Check for vulnerabilities in your components",
                  disableProxyOutput: true,
                             example: "//Check for vulnerabilities in your components|analyze")]
    public class AnalyzeCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            ConsoleService.Service.Clear();
            var teams = FilterService.Service.GetTeams(Configuration.ThirdEyeAgent.Teams.ToList()).ToList();
            var selectedTeams = ListService.ListDialog("Chose Team", teams.Select(t => t.Name).ToList(), autoSelectIfOnlyOneItem: false);
            if (selectedTeams.Count <= 0) return Ok();
            var selectedTeam = teams[selectedTeams.First().Key];

            var workspaces = FilterService.Service.GetWorkspaces(Configuration.ThirdEyeAgent.Workspaces.ToList(), selectedTeam).ToList();
            var selectedWorkspaces = ListService.ListDialog("Chose Workspace", workspaces.Select(w => w.Name).ToList());
            if (selectedWorkspaces.Count <= 0) return Ok();
            var selectedWorkspace = workspaces[selectedWorkspaces.First().Key];
            
            var repositories = FilterService.Service.GetRepositories(selectedWorkspace.Id).ToList();
            var selectedRepositories = ListService.ListDialog("Chose Repository", repositories.Select(r => r.Name).ToList());
            if (selectedRepositories.Count <= 0) return Ok();
            var selectedRepository = repositories[selectedRepositories.First().Key];

            var filteredThirdPartyComponents = FilterService.Service.GetThirdPartyComponents(selectedRepository).ToList();

            WriteLine("");
            WriteHeadLine("Analyze begins, loading CVEs...");
            if(CveStorage.LoadedCveCount == 0) CveStorage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();

            var analyzer = new CveAnalyzeManager(this);
            var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();

            var components = analyzer.GetVulnerabilities(CveStorage.GetCveEntries(), filteredThirdPartyComponents, threshold);
            var selectedComponentCves = PresentationManager.DisplayVulnerableComponents(components);
            var selected = ListService.ListDialog("Choose a component to view details.", selectedComponentCves.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selected.Count <= 0) return Ok();
            var component = selectedComponentCves[selected.First().Key];
            var componentCve = PresentationManager.DisplayVulnerableComponent(component);
            if (componentCve != null)
            {
                var apiKey = Configuration.Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
                var cveFetcher = new CveFetcherManager(CveStorage, configuration.ThirdEyeAgent.Nvd, apiKey, this);
                var cve = cveFetcher.FetchCveDetailsAsync(componentCve.Id).Result;
                if(cve != null) PresentationManager.DisplayCveDetails(cve);
            }
            WriteLine("");
            var thirdPartyComponent = Storage.GetThirdPartyComponents().First(c => c.Name == component.Name && c.Version == component.Version);
            ProjectSearch(thirdPartyComponent, detailedSearch: true);
            return Ok();
        }
    }
}