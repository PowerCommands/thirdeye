﻿using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Data;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

public class AnalyzeSoftwareWorkflow(IConsoleWriter writer2, PowerCommandsConfiguration configuration) : AnalyzeWorkflowBase(writer2, configuration)
{
    public override void Run(params string[] args)
    {
        Load();
        GetVulnerableComponents(null, $"{args.FirstOrDefault() ?? ""}");
        ViewCveDetails(VulnerableComponents);
    }

    public override List<ComponentCve> GetVulnerableComponents(Repository? repository, string fileName = "")
    {
        var software = StorageService<SoftwareObjects>.Service.GetObject().Items.Select(c => new ThirdPartyComponent { Name = c.Name, Version = c.Version }).ToList();
        var analyzer = new CveAnalyzeManager(writer2);
        var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
        
        var components = string.IsNullOrEmpty(fileName) ?  analyzer.GetVulnerabilities(CveStorageService.Service.GetCveEntries(), software, threshold) : analyzer.GetVulnerabilities(fileName);
        
        VulnerableComponents = PresentationManager.DisplayVulnerableComponents(components);
        return VulnerableComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder).ToList();
    }
}