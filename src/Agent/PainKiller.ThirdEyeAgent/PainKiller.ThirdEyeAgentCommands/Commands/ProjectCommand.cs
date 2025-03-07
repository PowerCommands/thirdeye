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
            var projects =  ObjectStorage.GetProjects().GetFilteredProjects(Configuration.ThirdEyeAgent.Projects);
            var (key, _) = ListService.ListDialog("Choose project", projects.Select(p => $"{p.Name} {p.Id}").ToList()).FirstOrDefault();
            var selectedProject = projects[key];
            WriteSuccess(selectedProject.Name);
            var repository = ObjectStorage.GetRepositories().Where(r => r.ProjectId == selectedProject.Id).ToList();
            var (key2, _) = ListService.ListDialog("Choose repository", repository.Select(r => $"{r.Name} {r.RepositoryId}").ToList()).FirstOrDefault();
            var selectedRepo = repository[key2];
            var devProjects = ObjectStorage.GetDevProjects().Where(p => p.RepositoryId == selectedRepo.RepositoryId).ToList();
            PresentationManager.DisplayRepository(selectedRepo.Name, devProjects);
            return Ok();
        }
    }
}