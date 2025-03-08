using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Synchronise with your source code server",
                  disableProxyOutput: true,
                             example: "Synchronise with your source code server//|synchronise")]
    public class SynchroniseCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            SynchroniseProjects();
            ObjectStorage.ReLoad();
            return Ok();
        }
        private List<Project> SynchroniseProjects()
        {
            var allProjects = GitManager.GetProjects().ToList();
            var projects = new List<Project>();
            foreach (var project in Configuration.ThirdEyeAgent.Projects)
            {
                if (project == "*")
                {
                    projects.AddRange(allProjects);
                    break;
                }
                var foundProject = allProjects.FirstOrDefault(p => p.Name == project);
                if (foundProject != null) projects.Add(foundProject);
            }
            WriteSuccessLine($"Fetched {projects.Count} Projects.");
            var teams = GitManager.GetAllTeams().ToList();
            ObjectStorage.SaveTeams(teams);
            WriteSuccessLine($"Fetched {teams.Count} Teams and persisted to storage.");
            
            var allRepositories = new List<Repository>();
            var removeProjects = new List<Project>();   //No Git repository found or project without any repository
            var removeRepositories = new List<Repository>();   //No Git repository found or project without any repository
            var allDistinctComponents = new List<ThirdPartyComponent>();
            var allDevProjects = new List<DevProject>();

            var projectIterationCount = 0;
            foreach (var project in projects)
            {
                projectIterationCount++;
                WriteLine($"Synchronosing project {project.Name} ({projectIterationCount} of {projects.Count})");
                var repositories = GitManager.GetRepositories(project.Id).ToList();
                if (repositories.Count == 0)
                {
                    removeProjects.Add(project);
                    continue;
                }
                allRepositories.AddRange(repositories);
                var repositoryIterationCount = 0;
                foreach (var repository in repositories)
                {
                    repositoryIterationCount++;
                    WriteLine($"Synchronosing repo {repository.Name} ({repositoryIterationCount} of {repositories.Count})");
                    var files = GitManager.GetAllFilesInRepository(repository.RepositoryId).ToList();
                    if (files.Count == 0)
                    {
                        WriteLine($"Repo {repository.Name} has no files and will be removed.");
                        removeRepositories.Add(repository);
                        continue;
                    }
                    WriteLine($"Found {files.Count} files, that now will be analyzed to find projects and components...");
                    var analyzeRepo = AnalyzeManager.AnalyzeRepo(files, project.Id, repository.RepositoryId);
                    foreach (var thirdPartyComponent in analyzeRepo.ThirdPartyComponents.Where(thirdPartyComponent => !allDistinctComponents.Any(c => c.Name == thirdPartyComponent.Name && c.Version == thirdPartyComponent.Version))) allDistinctComponents.Add(thirdPartyComponent);
                    allDevProjects.AddRange(analyzeRepo.DevProjects);
                }
            }
            ObjectStorage.SaveThirdPartyComponents(allDistinctComponents);
            WriteSuccessLine($"Extracted {allDistinctComponents.Count} distinct components and persisted them to storage.");
            ObjectStorage.SaveDevProjects(allDevProjects);
            WriteSuccessLine($"Extracted {allDevProjects.Count} dev projects and persisted them to storage.");
            foreach (var repo in removeRepositories) allRepositories.Remove(repo);
            ObjectStorage.SaveRepositories(allRepositories);
            WriteSuccessLine($"Fetched {allRepositories.Count} repositories and persisted them to storage.");
            foreach (var project in removeProjects) projects.Remove(project);
            ObjectStorage.SaveProjects(projects);
            WriteSuccessLine($"{allRepositories.Count} projects persisted to storage.\n");
            return projects;
        }
    }
}