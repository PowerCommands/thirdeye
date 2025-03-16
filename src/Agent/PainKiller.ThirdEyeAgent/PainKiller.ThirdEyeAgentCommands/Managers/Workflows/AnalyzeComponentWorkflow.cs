using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

public class AnalyzeComponentWorkflow(IConsoleWriter writer, PowerCommandsConfiguration configuration) : AnalyzeWorkflowBase(writer, configuration)
{
    public override void Run(params string[] args)
    {
        Load();
        var doAnalyze = true;
        
        while (doAnalyze)
        {
            GetVulnerableComponents(null, $"{args.FirstOrDefault() ?? ""}");
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
}