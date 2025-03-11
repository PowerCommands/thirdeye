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
                var viewResult = DialogService.YesNoDialog("Do you want to view your workspaces?");
                if(!viewResult) return Ok();
            }
            
            DisableLog();
            var projects =  Storage.GetWorkspaces().GetFilteredProjects(Configuration.ThirdEyeAgent.Workspaces);
            var (key, _) = ListService.ListDialog("Choose workspace", projects.Select(p => $"{p.Name} {p.Id}").ToList()).FirstOrDefault();
            var selectedProject = projects[key];
            WriteSuccessLine($"\n{selectedProject.Name}");
            var repositories = Storage.GetRepositories().Where(r => r.WorkspaceId == selectedProject.Id).ToList();
            if(repositories.Count == 0)
            {
                WriteLine("No repositories found for this project.");
                return Ok();
            }
            var (key2, _) = ListService.ListDialog("Choose repository", repositories.Select(r => $"{r.Name} {r.RepositoryId}").ToList()).FirstOrDefault();
            var selectedRepo = repositories[key2];
            var devProjects = Storage.GetDevProjects().Where(p => p.RepositoryId == selectedRepo.RepositoryId).ToList();
            PresentationManager.DisplayRepository(selectedRepo.Name, devProjects);
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