using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;
public class SynchronisationManager(IGitManager gitManager, IObjectStorageManager storage, IFileAnalyzeManager analyzeManager, IConsoleWriter writer, ThirdEyeConfiguration configuration)
{
    public List<Project> InitializeOrganisation()
    {
        var allProjects = gitManager.GetProjects().ToList();
        var projects = new List<Project>();
        foreach (var project in configuration.Projects)
        {
            if (project == "*")
            {
                projects.AddRange(allProjects);
                break;
            }
            var foundProject = allProjects.FirstOrDefault(p => p.Name == project);
            if (foundProject != null) projects.Add(foundProject);
        }
        writer.WriteSuccessLine($"Fetched {projects.Count} Projects.");
        var teams = gitManager.GetAllTeams().ToList();
        storage.SaveTeams(teams);
        writer.WriteSuccessLine($"Fetched {teams.Count} Teams and persisted to storage.");

        var allRepositories = new List<Repository>();
        var removeProjects = new List<Project>();           
        var removeRepositories = new List<Repository>();    
        var allDistinctComponents = new List<ThirdPartyComponent>();
        var allDevProjects = new List<DevProject>();

        var projectIterationCount = 0;
        foreach (var project in projects)
        {
            projectIterationCount++;
            writer.WriteLine($"Synchronosing project {project.Name} ({projectIterationCount} of {projects.Count})");
            var repositories = gitManager.GetRepositories(project.Id).ToList();
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
                writer.WriteLine($"Synchronosing repo {repository.Name} ({repositoryIterationCount} of {repositories.Count})");
                var files = gitManager.GetAllFilesInRepository(repository.RepositoryId).ToList();
                if (files.Count == 0)
                {
                    writer.WriteLine($"Repo {repository.Name} has no files and will be removed.");
                    removeRepositories.Add(repository);
                    continue;
                }
                writer.WriteLine($"Found {files.Count} files, that now will be analyzed to find projects and components...");
                var analyzeRepo = analyzeManager.AnalyzeRepo(files, project.Id, repository.RepositoryId);
                foreach (var thirdPartyComponent in analyzeRepo.ThirdPartyComponents.Where(thirdPartyComponent => !allDistinctComponents.Any(c => c.Name == thirdPartyComponent.Name && c.Version == thirdPartyComponent.Version))) allDistinctComponents.Add(thirdPartyComponent);
                allDevProjects.AddRange(analyzeRepo.DevProjects);
            }
        }
        storage.SaveThirdPartyComponents(allDistinctComponents);
        writer.WriteSuccessLine($"Extracted {allDistinctComponents.Count} distinct components and persisted them to storage.");
        storage.SaveDevProjects(allDevProjects);
        writer.WriteSuccessLine($"Extracted {allDevProjects.Count} dev projects and persisted them to storage.");
        foreach (var repo in removeRepositories) allRepositories.Remove(repo);
        storage.SaveRepositories(allRepositories);
        writer.WriteSuccessLine($"Fetched {allRepositories.Count} repositories and persisted them to storage.");
        foreach (var project in removeProjects) projects.Remove(project);
        storage.SaveProjects(projects);
        writer.WriteSuccessLine($"{projects.Count} projects persisted to storage.\n");
        return projects;
    }
    public void UpdateOrganisation()
    {
        var allGitProjects = gitManager.GetProjects().ToList();
        var allProjects = new List<Project>();
        foreach (var project in configuration.Projects)
        {
            if (project == "*")
            {
                allProjects.AddRange(allGitProjects);
                break;
            }
            var foundProject = allGitProjects.FirstOrDefault(p => p.Name == project);
            if (foundProject != null) allProjects.Add(foundProject);
        }
        writer.WriteSuccessLine($"Fetched {allProjects.Count} Projects.");
        var teams = gitManager.GetAllTeams().ToList();
        storage.SaveTeams(teams);
        writer.WriteSuccessLine($"Fetched {teams.Count} Teams and persisted to storage.");
        foreach (var project in allProjects)
        {
            storage.InsertOrUpdateProject(project);
            var gitRepos = gitManager.GetRepositories(project.Id);
            foreach (var repo in gitRepos)
            {
                var dbRepo = storage.GetRepositories().FirstOrDefault(r => r.RepositoryId == repo.RepositoryId);
                if (dbRepo != null && dbRepo.MainBranch?.CommitId == repo.MainBranch?.CommitId)
                {
                    writer.WriteLine($"Repo {repo.Name} is up to date.");
                    continue;
                }
                storage.UpdateOrInsertRepository(repo);
                writer.WriteLine($"Repo {repo.Name} has been updated.");
                var files = gitManager.GetAllFilesInRepository(repo.RepositoryId).ToList();
                if (files.Count == 0)
                {
                    writer.WriteLine($"Repo {repo.Name} has no files and will be removed.");
                    storage.RemoveRepository(repo.RepositoryId);
                    continue;
                }
                writer.WriteLine($"Found {files.Count} files, that now will be analyzed to find projects and components...");
                var analyzeRepo = analyzeManager.AnalyzeRepo(files, project.Id, repo.RepositoryId);
                foreach (var thirdPartyComponent in analyzeRepo.ThirdPartyComponents) storage.InsertComponent(thirdPartyComponent);
                var insertCount = storage.InsertDevProjects(analyzeRepo.DevProjects);
                writer.WriteLine($"{insertCount} DevProjects updated");
            }
        }
        CleanUpProjectsNotIncludedAnymore(allProjects);
        CleanUpRemovedComponents();
        writer.WriteSuccessLine("Synchronisation done!");
    }
    private void CleanUpProjectsNotIncludedAnymore(List<Project> allProjects)
    {
        var storageProjects = storage.GetProjects();
        foreach (var project in storageProjects)
        {
            if (allProjects.All(p => p.Id != project.Id))
            {
                storage.RemoveProject(project.Id);
                writer.WriteLine($"{project.Name} has been removed from storage because is no longer included in the configured selection of projects.");
            }
        }
    }
    private void CleanUpRemovedComponents()
    {
        storage.ReLoad();
        var thirdPartyComponents = storage.GetThirdPartyComponents();
        var repos = storage.GetRepositories();
        var updatedThirdPartyComponents = new List<ThirdPartyComponent>();
        foreach (var component in thirdPartyComponents) if(repos.Any(r => r.MainBranch?.CommitId == component.CommitId)) updatedThirdPartyComponents.Add(component);
        if (thirdPartyComponents.Count != updatedThirdPartyComponents.Count)
        {
            writer.WriteLine("Some components has been removed from storage because they are no longer included in any selected repository branch.");
            writer.WriteSuccessLine($"{updatedThirdPartyComponents.Count} component persisted to storage.\n");
            storage.SaveThirdPartyComponents(updatedThirdPartyComponents);
            return;
        }
    }
}