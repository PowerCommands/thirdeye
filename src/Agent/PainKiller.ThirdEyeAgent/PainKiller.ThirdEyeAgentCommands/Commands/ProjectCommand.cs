using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle projects",
                             options: "sync-with-host",
                  disableProxyOutput: true,
                             example: "//List all projects|project")]
    public class ProjectCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var projects = HasOption("sync-with-host") ? SyncWithServer() : DbManager.GetProjects().GetFilteredProjects(Configuration.ThirdEyeAgent.Projects);
            var (key, _) = ListService.ListDialog("Choose project", projects.Select(p => $"{p.Name, 50} {p.Id}").ToList()).First();
            var selectedProject = projects[key];
            WriteSuccess(selectedProject.Name);
            var repository = DbManager.GetRepositories().Where(r => r.ProjectId == selectedProject.Id).ToList();
            var (key2, _) = ListService.ListDialog("Choose repository", repository.Select(r => $"{r.Name, 50} {r.RepositoryId}").ToList()).First();
            var selectedRepo = repository[key2];
            var files = AdsManager.GetAllFilesInRepository(selectedRepo.RepositoryId).ToList();
            WriteSeparatorLine();
            var components = AnalyzeManager.AnalyzeFiles(files);
            foreach (var thirdPartyComponent in components) WriteCodeExample(thirdPartyComponent.Name, thirdPartyComponent.Version);
            
            return Ok();
        }
        private List<Project> SyncWithServer()
        {
            var projects = AdsManager.GetProjects().ToList();
            WriteSuccessLine($"{projects.Count} Projects synced with host.");
            WriteSeparatorLine();
            var teams = AdsManager.GetAllTeams().ToList();
            DbManager.SaveTeams(teams);
            WriteSuccessLine($"{teams.Count} Teams synced with host.");
            WriteSeparatorLine();
            var allRepositories = new List<Repository>();
            var removeProjects = new List<Project>();
            foreach (var project in projects)
            {
                var repositories = AdsManager.GetRepositories(project.Id).ToList();
                if(repositories.Count == 0) removeProjects.Add(project);
                allRepositories.AddRange(repositories);
            }
            DbManager.SaveRepositories(allRepositories);
            foreach (var project in removeProjects) projects.Remove(project);
            DbManager.SaveProjects(projects);
            WriteSuccessLine($"{allRepositories.Count} Repositories synced with host.\n");
            return projects;
        }
    }
}