using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Services;
public class FilterService
{ 
    private IObjectStorageService StorageService => ObjectStorageService.Service;

    private static readonly Lazy<FilterService> Lazy = new(() => new FilterService());
    public static FilterService Service => Lazy.Value;
    private FilterService(){}
    public IEnumerable<Workspace> GetWorkspaces(List<string> workspaceIds, Team team)
    {
        if (workspaceIds.Count == 0) return [];
        if (workspaceIds.First() == "*") return StorageService.GetWorkspaces();
        return StorageService.GetWorkspaces().Where(w => workspaceIds.Contains(w.Id.ToString()) && team.WorkspaceIds.Contains(w.Id)).ToList();
    }
    public IEnumerable<Team> GetTeams(List<string> teamIds)
    {
        if (teamIds.Count == 0) return [];
        if(teamIds.First() == "*") return StorageService.GetTeams();
        return StorageService.GetTeams().Where(t => teamIds.Contains(t.Id.ToString())).ToList();
    }
    public IEnumerable<Repository> GetRepositories(Guid workspaceId) => StorageService.GetRepositories().Where(r => r.WorkspaceId == workspaceId);
    public IEnumerable<ThirdPartyComponent> GetThirdPartyComponents(Repository repository) => StorageService.GetThirdPartyComponents().Where(c => c.CommitId == repository.MainBranch?.CommitId).ToList();
}