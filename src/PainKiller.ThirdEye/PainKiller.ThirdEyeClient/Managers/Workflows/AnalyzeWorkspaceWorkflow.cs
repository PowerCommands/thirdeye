﻿using PainKiller.ThirdEyeClient.Configuration;

namespace PainKiller.ThirdEyeClient.Managers.Workflows;

public class AnalyzeWorkspaceWorkflow(IConsoleWriter writer, CommandPromptConfiguration configuration) : AnalyzeWorkflowBase(writer, configuration)
{
    public override void Run(params string[] args)
    {
        Load();
        SelectTeam();
        SelectWorkspace(Team ?? ObjectStorageService.Service.GetTeams().First());
        var doAnalyze = true;
        while (doAnalyze)
        {
            SelectRepository(Workspace?.Id ?? ObjectStorageService.Service.GetWorkspaces().First().Id);
            GetVulnerableComponents(Repository);
            if (VulnerableComponents.Count == 0) break;
            var component = ViewCveDetails(VulnerableComponents);
            if (component != null)
            {
                if (CveEntry != null) SaveFinding(component,  CveEntry);

                var viewWorkspace = DialogService.YesNoDialog("Do you want to view where the vulnerable components is used?");
                if(viewWorkspace) WorkspaceSearch(new ThirdPartyComponent{Name = component.Name, Version = component.Version}, true);
            }
            doAnalyze =DialogService.YesNoDialog("Chose another repo (y) or quit? (not y)");
        }
    }
}