using PainKiller.ThirdEyeAgentCommands.Extensions;
using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle workspaces (Projects in ADS)",
                  disableProxyOutput: true,
                             options: "init|update",
                             example: "//List all your configured workspaces|workspace")]
    public class WorkspaceCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            if (Options.Count > 0)
            {
                Init();
                PauseService.Pause(3);
            }
            
            DisableLog();
            var workspaces =  Storage.GetWorkspaces().GetFilteredWorkspaces(Configuration.ThirdEyeAgent.Workspaces);
            var (key, _) = ListService.ListDialog("Choose workspace", workspaces.Select(p => $"{p.Name} {p.Id}").ToList()).FirstOrDefault();
            var selectedWorkspace = workspaces[key];
            WriteSuccessLine($"\n{selectedWorkspace.Name}");
            var repositories = Storage.GetRepositories().Where(r => r.WorkspaceId == selectedWorkspace.Id).ToList();
            if(repositories.Count == 0)
            {
                WriteLine("No repositories found for this project.");
                return Ok();
            }
            var (key2, _) = ListService.ListDialog("Choose repository", repositories.Select(r => $"{r.Name} {r.RepositoryId}").ToList()).FirstOrDefault();
            var selectedRepo = repositories[key2];
            var projects = Storage.GetProjects().Where(p => p.RepositoryId == selectedRepo.RepositoryId).ToList();
            PresentationManager.DisplayRepository(selectedRepo.Name, projects);
            EnableLog();
            return Ok();
        }
        private void Init()
        {
            var workspaceManager = new WorkspaceManager(GitManager, Storage, AnalyzeManager, this, Configuration.ThirdEyeAgent);
            if(HasOption("init")) workspaceManager.InitializeOrganization();
            else if(HasOption("update")) workspaceManager.UpdateOrganization();
            Storage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
        }
    }
}