using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

public class AnalyzeProjectWorkflow(IConsoleWriter writer2, PowerCommandsConfiguration configuration) : AnalyzeWorkflowBase(writer2, configuration)
{
    public override void Run(params string[] args)
    {
        Load();
        var projectName = args.First();
        VulnerableComponents = GetVulnerableComponents(null, projectName);
        UpdateInfoPanel();
        if (VulnerableComponents.Count == 0)
        {
            writer2.WriteSuccessLine($"No vulnerabilities found in {projectName}!");
            return;
        }
        var component = ViewCveDetails(VulnerableComponents);
        if (component != null)
        {
            var viewWorkspace = DialogService.YesNoDialog("Do you want to view where the vulnerable components is used?");
            if(viewWorkspace) WorkspaceSearch(new ThirdPartyComponent{Name = component.Name, Version = component.Version}, true);
        }
    }
    public override List<ComponentCve> GetVulnerableComponents(Repository? repository, string projectName = "")
    {
        var filteredThirdPartyComponents = repository == null ? ObjectStorageService.Service.GetThirdPartyComponents() : FilterService.Service.GetThirdPartyComponents(repository, projectName).ToList();
        var analyzer = new CveAnalyzeManager(writer2);
        var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
        var components = analyzer.GetVulnerabilities(CveStorageService.Service.GetCveEntries(), filteredThirdPartyComponents, threshold);
        VulnerableComponents = PresentationManager.DisplayVulnerableComponents(components);
        return VulnerableComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder).ToList();
    }
}