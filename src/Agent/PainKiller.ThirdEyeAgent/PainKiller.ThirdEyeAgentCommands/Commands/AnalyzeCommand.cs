using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Managers;

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
            WriteHeadLine("Analyze begins, loading CVEs...");
            if(CveStorage.LoadedCveCount == 0) CveStorage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();

            var analyzer = new CveAnalyzeManager(this);
            var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
            var components = analyzer.GetVulnerabilities(CveStorage.GetCveEntries(), Storage.GetThirdPartyComponents(),threshold);
            var selectedComponentCves = PresentationManager.DisplayVulnerableComponents(components);
            
            var selected = ListService.ListDialog("Choose a component to view details.", selectedComponentCves.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selected.Count <= 0) return Ok();
            var component = selectedComponentCves[selected.First().Key];
            WriteLine("");
            var thirdPartyComponent = Storage.GetThirdPartyComponents().First(c => c.Name == component.Name && c.Version == component.Version);
            ProjectSearch(thirdPartyComponent, detailedSearch: true);
            return Ok();
        }
    }
}