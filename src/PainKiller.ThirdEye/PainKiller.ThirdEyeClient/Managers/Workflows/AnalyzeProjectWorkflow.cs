using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Configuration;

namespace PainKiller.ThirdEyeClient.Managers.Workflows;

public class AnalyzeProjectWorkflow(IConsoleWriter writer2, CommandPromptConfiguration configuration) : AnalyzeWorkflowBase(writer2, configuration)
{
    public override void Run(params string[] args)
    {
        Load();
        var projectName = args.First();
        VulnerableComponents = GetVulnerableComponents(projectName);
        if (VulnerableComponents.Count == 0)
        {
            writer2.WriteSuccessLine($"No vulnerabilities found in {projectName}!");
            return;
        }
        var component = ViewCveDetails(VulnerableComponents);
        if (component != null)
        {
            if (CveEntry != null) SaveFinding(component,  CveEntry);

            var viewWorkspace = DialogService.YesNoDialog("Do you want to view where the vulnerable components is used?");
            if(viewWorkspace) WorkspaceSearch(new ThirdPartyComponent{Name = component.Name, Version = component.Version}, true);
        }
    }
    public List<ComponentCve> GetVulnerableComponents(string projectName = "")
    {
        var filteredThirdPartyComponents = FilterService.Service.GetThirdPartyComponents(projectName).ToList();
        var analyzer = new CveAnalyzeManager(writer2, configuration);
        var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
        var components = analyzer.GetVulnerabilities(CveStorageService.Service.GetCveEntries(), filteredThirdPartyComponents, threshold);
        VulnerableComponents = PresentationManager.DisplayVulnerableComponents(components);
        return VulnerableComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder).ToList();
    }
}