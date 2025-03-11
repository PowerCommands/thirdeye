using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle workspaces (Projects in ADS)",
                  disableProxyOutput: true,
                             example: "//List all your configured workspaces|workspace")]
    public class WorkspaceCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var projects =  Storage.GetProjects().GetFilteredProjects(Configuration.ThirdEyeAgent.Projects);
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
    }
}