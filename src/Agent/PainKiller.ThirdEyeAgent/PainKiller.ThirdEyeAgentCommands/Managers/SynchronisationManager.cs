using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;
public class SynchronisationManager(IGitManager gitManager, IObjectStorageManager storage, IFileAnalyzeManager analyzeManager, IConsoleWriter writer, ThirdEyeConfiguration configuration)
{
    public List<Project> InitializeProjects()
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
    public List<Project> UpdateProjects()
    {
        var syncProjects = new List<Project>();
        var syncRepos = new List<Repository>();
        var syncDevProjects = new List<DevProject>();

        var projects = storage.GetProjects();
        var repositories = storage.GetRepositories();
        var devProjects = storage.GetDevProjects();
        var allDistinctComponents = storage.GetThirdPartyComponents();
        
        var allProjects = gitManager.GetProjects().ToList();
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
        var teams = gitManager.GetAllTeams().ToList();
        storage.SaveTeams(teams);
        writer.WriteSuccessLine($"Fetched {teams.Count} Teams and persisted to storage.");
        foreach (var project in projects)
        {
            var newRepositories = gitManager.GetRepositories(project.Id).ToList();
            foreach (var repo in newRepositories)
            {
                if (repositories.All(r => r.RepositoryId != repo.RepositoryId)) syncRepos.Add(repo);    //New repository must be added
                var files = gitManager.GetAllFilesInRepository(repo.RepositoryId).ToList();
                if(string.IsNullOrEmpty(files.FirstOrDefault()?.CommitId)) continue;    //No files in repository should not be analyzed
                if (repositories.Any(r => r.MainBranch?.CommitId == repo.MainBranch?.CommitId))
                {
                    syncRepos.Add(repo);    //Main branch has not changed
                    var repoDevProjects = devProjects.Where(d => d.RepositoryId == repo.RepositoryId).ToList();
                    syncDevProjects.AddRange(repoDevProjects);
                    writer.WriteLine($"Repository {repo.Name} has no changes.");
                    continue;
                }
                writer.WriteLine($"Repository {repo.Name} needs to be analyzed again.");
                writer.WriteLine($"Found {files.Count} files, that now will be analyzed to find projects and components...");
                var analyzeRepo = analyzeManager.AnalyzeRepo(files, project.Id, repo.RepositoryId);
                foreach (var thirdPartyComponent in analyzeRepo.ThirdPartyComponents.Where(thirdPartyComponent => !allDistinctComponents.Any(c => c.Name == thirdPartyComponent.Name && c.Version == thirdPartyComponent.Version))) allDistinctComponents.Add(thirdPartyComponent);
                syncDevProjects.AddRange(analyzeRepo.DevProjects);
            }
            syncProjects.Add(project);
        }
        storage.SaveThirdPartyComponents(allDistinctComponents);
        writer.WriteSuccessLine($"Synchronised {allDistinctComponents.Count} distinct components and persisted them to storage.");
        storage.SaveDevProjects(syncDevProjects);
        writer.WriteSuccessLine($"Synchronised {syncDevProjects.Count} dev projects and persisted them to storage.");
        
        storage.SaveRepositories(syncRepos);
        writer.WriteSuccessLine($"Synchronised {syncRepos.Count} repositories and persisted them to storage.");
        storage.SaveProjects(projects);
        writer.WriteSuccessLine($"{projects.Count} projects persisted to storage.\n");
        CleanUpRemovedComponents();
        return syncProjects;
    }
    private void CleanUpRemovedComponents()
    {
        storage.ReLoad();
        var thirdPartyComponents = storage.GetThirdPartyComponents();
        var repos = storage.GetRepositories();
        var updatedThirdPartyComponents = new List<ThirdPartyComponent>();
        foreach (var component in thirdPartyComponents) if(repos.Any(r => r.MainBranch?.CommitId == component.CommitId)) updatedThirdPartyComponents.Add(component);
        if(thirdPartyComponents.Count != UpdateProjects().Count) writer.WriteLine("Some components has been removed from storage because they are no longer included in any repository branch.");
        writer.WriteSuccessLine($"{updatedThirdPartyComponents.Count} component persisted to storage.\n");
        storage.SaveThirdPartyComponents(updatedThirdPartyComponents);
    }
}