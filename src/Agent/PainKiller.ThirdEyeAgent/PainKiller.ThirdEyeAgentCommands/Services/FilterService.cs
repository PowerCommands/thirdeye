using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Services;
public class FilterService
{
    private IObjectStorageService StorageService => ObjectStorageService.Service;
    private static readonly Lazy<FilterService> Lazy = new(() => new FilterService());
    public static FilterService Service => Lazy.Value;
    private FilterService(){}
    public IEnumerable<Workspace> GetWorkspaces(List<string> workspaceNames, Team team)
    {
        if (workspaceNames.Count == 0) return [];
        if (workspaceNames.First() == "*") return StorageService.GetWorkspaces();
        return StorageService.GetWorkspaces().Where(w => workspaceNames.Any(name => w.Name.ToLower().Contains(name.ToLower())) && team.WorkspaceIds.Contains(w.Id)).ToList();
    }
    public IEnumerable<Team> GetTeams(List<string> teamNames)
    {
        if (teamNames.Count == 0) return [];
        if(teamNames.First() == "*") return StorageService.GetTeams();
        return StorageService.GetTeams().Where(t => teamNames.Any(name => t.Name.ToLower().Contains(name.ToLower()))).ToList();
    }
    public IEnumerable<Repository> GetRepositories(Guid workspaceId) => StorageService.GetRepositories().Where(r => r.WorkspaceId == workspaceId);
    public IEnumerable<ThirdPartyComponent> GetThirdPartyComponents(Repository repository)
    {
        var retVal = new List<ThirdPartyComponent>();
        var projects = StorageService.GetProjects().Where(p => p.RepositoryId == repository.RepositoryId);
        foreach (var project in projects) retVal.AddRange(project.Components);
        return retVal;
    }
}