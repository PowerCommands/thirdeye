using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Services;
public class FilterService
{ 
    private IObjectStorageService StorageService => ObjectStorageService.Service;

    private static readonly Lazy<FilterService> Lazy = new(() => new FilterService());
    public static FilterService Service => Lazy.Value;
    private FilterService(){}
    public IEnumerable<Workspace> GetWorkspaces(List<string> workspaceIds)
    {
        if (workspaceIds.Count == 0) return [];
        return StorageService.GetWorkspaces().Where(w => workspaceIds.Contains(w.Id.ToString())).ToList();
    }
    public IEnumerable<ThirdPartyComponent> GetThirdPartyComponents(List<Workspace> workspaces, List<Team> filteredTams, List<Repository> filteredRepositories)
    {
        var retVal = new List<ThirdPartyComponent>();
        var teams = new List<Team>();
        foreach (var workspace in workspaces)
        {
            var team = filteredTams.FirstOrDefault(t => t.WorkspaceIds.Contains(workspace.Id));
            if (team != null || ( team != null && filteredTams.Count == 0)) teams.Add(team);
        }
        var repositories = new List<Repository>();
        foreach (var team in teams)
        {
            var repository = filteredRepositories.FirstOrDefault(r => team.WorkspaceIds.Any(w => w == r.WorkspaceId));
            if (repository != null || (repository != null && filteredRepositories.Count == 0)) repositories.Add(repository);
        }
        foreach (var repository in repositories)
        {
            var components = StorageService.GetThirdPartyComponents().Where(c => c.CommitId == repository.MainBranch?.CommitId).ToList();
            retVal.AddRange(components);
        }
        return retVal;
    }
    
}