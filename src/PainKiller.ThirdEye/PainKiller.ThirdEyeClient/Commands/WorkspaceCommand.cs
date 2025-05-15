using PainKiller.ThirdEyeClient.Managers;
using PainKiller.ThirdEyeClient.Managers.Workflows;

namespace PainKiller.ThirdEyeClient.Commands;
[CommandDesign(description: "Handle workspaces (Projects in ADS)",
    arguments: [],
    options: ["analyze", "init", "update"],
    examples: ["//List all your configured workspaces", "workspace"])]
public class WorkspaceCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        if (input.HasOption("init")) Init();
        else if (input.HasOption("update")) Update();
        else if (input.HasOption("analyze")) Analyze();

        Storage.ReLoad();

        var viewWorkspace = DialogService.YesNoDialog("Do you want to view your workspaces?");
        if (!viewWorkspace) return Ok();

        var workspaces = Storage.GetWorkspaces().GetFilteredWorkspaces(Configuration.ThirdEye.Workspaces);
        var selected = ListService.ListDialog("Choose workspace", workspaces.Select(p => $"{p.Name} {p.Id}").ToList());

        if (selected.Count == 0) return Ok();
        var (key, _) = selected.First();
        var selectedWorkspace = workspaces[key];
        Writer.WriteSuccessLine($"\n{selectedWorkspace.Name}");

        var repositories = Storage.GetRepositories().Where(r => r.WorkspaceId == selectedWorkspace.Id).ToList();
        if (repositories.Count == 0)
        {
            Writer.WriteLine("No repositories found for this project.");
            return Ok();
        }

        var selectedRepo = ListService.ListDialog("Choose repository", repositories.Select(r => $"{r.Name} {r.RepositoryId}").ToList()).FirstOrDefault();
        if (selectedRepo.Equals(default)) return Ok();
        
        var (key2, _) = selectedRepo;
        var repo = repositories[key2];
        var projects = Storage.GetProjects().Where(p => p.RepositoryId == repo.RepositoryId).ToList();
        PresentationManager.DisplayRepository(repo.Name, projects);
        
        return Ok();
    }

    private void Init()
    {
        var workspaceManager = new WorkspaceManager(GitManager, Storage, AnalyzeManager, Writer, Configuration.ThirdEye);
        workspaceManager.InitializeOrganization();
    }

    private void Update()
    {
        var workspaceManager = new WorkspaceManager(GitManager, Storage, AnalyzeManager, Writer, Configuration.ThirdEye);
        workspaceManager.UpdateOrganization();
    }

    private void Analyze()
    {
        var workflow = new AnalyzeWorkspaceWorkflow(Writer, Configuration);
        workflow.Run();
    }
}