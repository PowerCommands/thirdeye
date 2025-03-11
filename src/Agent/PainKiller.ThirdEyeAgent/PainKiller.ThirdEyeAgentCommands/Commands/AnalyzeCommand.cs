using PainKiller.ThirdEyeAgentCommands.Data;
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
            var components = analyzer.GetVulnerabilities(CveStorage.GetCveEntries(), Storage.GetThirdPartyComponents(), StorageService<SoftwareObjects>.Service.GetObject().Software, threshold);
            PresentationManager.DisplayVulnerableComponents(components);
            return Ok();
        }
    }
}