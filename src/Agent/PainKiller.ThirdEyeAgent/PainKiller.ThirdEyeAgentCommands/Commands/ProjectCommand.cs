using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle projects",
                  disableProxyOutput: true,
                             example: "//List all projects|project")]
    public class ProjectCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var workspaces =  Storage.GetWorkspaces().GetFilteredProjects(Configuration.ThirdEyeAgent.Workspaces);
            var (key, _) = ListService.ListDialog("Choose workspace", workspaces.Select(p => $"{p.Name} {p.Id}").ToList()).FirstOrDefault();
            var selectedProject = workspaces[key];
            WriteSuccessLine($"\n{selectedProject.Name}");
            var repositories = Storage.GetRepositories().Where(r => r.WorkspaceId == selectedProject.Id).ToList();
            if(repositories.Count == 0)
            {
                WriteLine("No repositories found for this workspace.");
                return Ok();
            }
            var (key2, _) = ListService.ListDialog("Choose repository", repositories.Select(r => $"{r.Name} {r.RepositoryId}").ToList()).FirstOrDefault();
            var selectedRepo = repositories[key2];
            var projects = Storage.GetProjects().Where(p => p.RepositoryId == selectedRepo.RepositoryId).ToList();
            PresentationManager.DisplayRepository(selectedRepo.Name, projects);
            EnableLog();
            return Ok();
        }
    }
}