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
            var projects =  ObjectStorage.GetProjects().GetFilteredProjects(configuration.ThirdEyeAgent.Projects);
            var (key, _) = ListService.ListDialog("Choose project", projects.Select(p => $"{p.Name} {p.Id}").ToList()).FirstOrDefault();
            var selectedProject = projects[key];
            WriteSuccessLine($"\n{selectedProject.Name}");
            var repositories = ObjectStorage.GetRepositories().Where(r => r.ProjectId == selectedProject.Id).ToList();
            if(repositories.Count == 0)
            {
                WriteLine("No repositories found for this project.");
                return Ok();
            }
            var (key2, _) = ListService.ListDialog("Choose repository", repositories.Select(r => $"{r.Name} {r.RepositoryId}").ToList()).FirstOrDefault();
            var selectedRepo = repositories[key2];
            var devProjects = ObjectStorage.GetDevProjects().Where(p => p.RepositoryId == selectedRepo.RepositoryId).ToList();
            PresentationManager.DisplayRepository(selectedRepo.Name, devProjects);
            EnableLog();
            return Ok();
        }
    }
}