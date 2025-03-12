using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Managers;
public class WorkspaceManager(IGitManager gitManager, IObjectStorageService storage, IFileAnalyzeManager analyzeManager, IConsoleWriter writer, ThirdEyeConfiguration configuration)
{
    public List<Workspace> InitializeOrganization()
    {
        var allWorkspaces = gitManager.GetWorkspaces().ToList();
        var workspaces = configuration.ConfigurationFilter(allWorkspaces, configuration.Workspaces, p => p.Name).ToList();
        writer.WriteSuccessLine($"Fetched {workspaces.Count} workspace(s).");
        var allTeams = gitManager.GetAllTeams().ToList();
        var teams = configuration.ConfigurationFilter(allTeams, configuration.Teams, t => t.Name).ToList();
        storage.SaveTeams(teams);
        writer.WriteSuccessLine($"Fetched {teams.Count} Teams and persisted to storage.");

        var allRepositories = new List<Repository>();
        var removeWorkspaces = new List<Workspace>();           
        var removeRepositories = new List<Repository>();    
        var allDistinctComponents = new List<ThirdPartyComponent>();
        var allProjects = new List<Project>();

        var workspaceIterationCount = 0;
        foreach (var workspace in workspaces)
        {
            workspaceIterationCount++;
            writer.WriteLine($"Synchronosing workspace {workspace.Name} ({workspaceIterationCount} of {workspaces.Count})");
            Console.CursorTop -= 1;
            var repositories = gitManager.GetRepositories(workspace.Id).ToList();
            if (repositories.Count == 0)
            {
                removeWorkspaces.Add(workspace);
                continue;
            }
            allRepositories.AddRange(repositories);
            var repositoryIterationCount = 0;
            foreach (var repository in repositories)
            {
                repositoryIterationCount++;
                writer.WriteLine($"Synchronosing repo {repository.Name} ({repositoryIterationCount} of {repositories.Count})");
                Console.CursorTop -= 1;
                var files = gitManager.GetAllFilesInRepository(repository.RepositoryId).ToList();
                if (files.Count == 0)
                {
                    writer.WriteLine($"Repo {repository.Name} has no files and will be removed.");
                    Console.CursorTop -= 1;
                    removeRepositories.Add(repository);
                    continue;
                }
                writer.WriteLine($"Found {files.Count} files, that now will be analyzed to find projects and components...");
                Console.CursorTop -= 1;
                var analyzeRepo = analyzeManager.AnalyzeRepo(files, workspace.Id, repository.RepositoryId);
                foreach (var thirdPartyComponent in analyzeRepo.ThirdPartyComponents.Where(thirdPartyComponent => !allDistinctComponents.Any(c => c.Name == thirdPartyComponent.Name && c.Version == thirdPartyComponent.Version))) allDistinctComponents.Add(thirdPartyComponent);
                allProjects.AddRange(analyzeRepo.Projects);
            }
        }
        storage.SaveThirdPartyComponents(allDistinctComponents);
        writer.WriteSuccessLine($"Extracted {allDistinctComponents.Count} distinct components and persisted them to storage.");
        storage.SaveProjects(allProjects);
        writer.WriteSuccessLine($"Extracted {allProjects.Count} projects and persisted them to storage.");
        foreach (var repo in removeRepositories) allRepositories.Remove(repo);
        storage.SaveRepositories(allRepositories);
        writer.WriteSuccessLine($"Fetched {allRepositories.Count} repositories and persisted them to storage.");
        foreach (var project in removeWorkspaces) workspaces.Remove(project);
        storage.SaveWorkspace(workspaces);
        writer.WriteSuccessLine($"{workspaces.Count} workspaces persisted to storage.\n");
        return workspaces;
    }
    public void UpdateOrganization()
    {
        var allGitWorkspaces = gitManager.GetWorkspaces().ToList();
        var allWorkspaces = configuration.ConfigurationFilter(allGitWorkspaces, configuration.Workspaces, p => p.Name).ToList();
        
        writer.WriteSuccessLine($"Fetched {allWorkspaces.Count} workspace(s).");
        var allTeams = gitManager.GetAllTeams().ToList();
        var teams = configuration.ConfigurationFilter(allTeams, configuration.Teams, t => t.Name).ToList();
        storage.SaveTeams(teams);
        writer.WriteSuccessLine($"Fetched {teams.Count} Teams and persisted to storage.");
        foreach (var project in allWorkspaces)
        {
            storage.InsertOrUpdateWorkspace(project);
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
                var insertCount = storage.InsertProjects(analyzeRepo.Projects);
                writer.WriteLine($"{insertCount} Projects updated");
            }
        }
        CleanUpWorkspacesNotIncludedAnymore(allWorkspaces);
        CleanUpRemovedComponents();
        writer.WriteSuccessLine("Synchronisation done!");
    }
    private void CleanUpWorkspacesNotIncludedAnymore(List<Workspace> allProjects)
    {
        var storageWorkspaces = storage.GetWorkspaces();
        foreach (var workspace in storageWorkspaces)
        {
            if (allProjects.All(p => p.Id != workspace.Id))
            {
                storage.RemoveWorkspace(workspace.Id);
                writer.WriteLine($"{workspace.Name} has been removed from storage because is no longer included in the configured selection of workspaces.");
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