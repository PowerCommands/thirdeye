using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Bootstrap;
using PainKiller.ThirdEyeClient.Data;
using PainKiller.ThirdEyeClient.DomainObjects;
using PainKiller.ThirdEyeClient.Enums;
using PainKiller.ThirdEyeClient.Services;

namespace PainKiller.ThirdEyeClient.Managers.Workflows;

public class AnalyzeSoftwareWorkflow(IConsoleWriter writer, CommandPromptConfiguration configuration) : AnalyzeWorkflowBase(writer, configuration)
{ 
    public override void Run(params string[] args)
    {
        Load();
        GetVulnerableComponents(null, $"{args.FirstOrDefault() ?? ""}");
        var componentCve = ViewCveDetails(VulnerableComponents);
        if (componentCve != null && CveEntry != null) SaveFinding(componentCve,  CveEntry);
    }
    public override List<ComponentCve> GetVulnerableComponents(Repository? repository, string fileName = "")
    {
        var software = StorageService<SoftwareObjects>.Service.GetObject().Items.Select(c => new ThirdPartyComponent { Name = c.Name, Version = c.Version }).ToList();
        var analyzer = new CveAnalyzeManager(writer, configuration);
        var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
        
        var components = string.IsNullOrEmpty(fileName) ?  analyzer.GetVulnerabilities(CveStorageService.Service.GetCveEntries(), software, threshold) : analyzer.GetVulnerabilities(fileName);
        
        VulnerableComponents = PresentationManager.DisplayVulnerableComponents(components);
        return VulnerableComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder).ToList();
    }
}