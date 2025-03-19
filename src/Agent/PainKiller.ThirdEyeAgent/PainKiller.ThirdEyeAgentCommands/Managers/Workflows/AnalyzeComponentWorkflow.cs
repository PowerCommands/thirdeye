using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

public class AnalyzeComponentWorkflow(IConsoleWriter writer, PowerCommandsConfiguration configuration, ThirdPartyComponent thirdPartyComponent) : AnalyzeWorkflowBase(writer, configuration)
{
    public override void Run(params string[] args)
    {
        Load();
        var doAnalyze = true;
        
        while (doAnalyze)
        {
            GetVulnerableComponents();
            var component = ViewCveDetails(VulnerableComponents);
            if (component != null)
            {
                if (CveEntry != null) SaveFinding(component,  CveEntry);

                var viewWorkspace = DialogService.YesNoDialog("Do you want to view where the vulnerable components is used?");
                if(viewWorkspace) WorkspaceSearch(new ThirdPartyComponent{Name = component.Name, Version = component.Version}, true);
            }
            doAnalyze =DialogService.YesNoDialog("Do one more analyze? (y) or quit? (not y)");
        }
    }
    public List<ComponentCve> GetVulnerableComponents()
    {
        var filteredThirdPartyComponents = thirdPartyComponent.Name == "*" ? ObjectStorageService.Service.GetThirdPartyComponents() : [thirdPartyComponent];
        var analyzer = new CveAnalyzeManager(writer);
        var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
        var components = analyzer.GetVulnerabilities(CveStorageService.Service.GetCveEntries(), filteredThirdPartyComponents, threshold);
        VulnerableComponents = PresentationManager.DisplayVulnerableComponents(components);
        return VulnerableComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder).ToList();
    }
}