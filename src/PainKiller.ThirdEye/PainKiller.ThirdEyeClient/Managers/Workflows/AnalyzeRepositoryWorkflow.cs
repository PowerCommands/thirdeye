using PainKiller.ThirdEyeClient.Configuration;

namespace PainKiller.ThirdEyeClient.Managers.Workflows;

public class AnalyzeRepositoryWorkflow(IConsoleWriter writer, CommandPromptConfiguration configuration) : AnalyzeWorkflowBase(writer, configuration)
{
    public override void Run(params string[] args)
    {
        var repoName = args.First();
        var repo = ObjectStorageService.Service.GetRepositories().First(r => r.Name == repoName);
        Load();
        GetVulnerableComponents(repo);
        if (VulnerableComponents.Count == 0)
        {
            writer.WriteSuccessLine($"No vulnerabilities found in {repoName}!");
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
}