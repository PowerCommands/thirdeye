using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IObjectStorageManager
{
    List<Team> GetTeams();
    List<Workspace> GetWorkspaces();
    bool RemoveWorkspace(Guid workspaceId);
    void InsertOrUpdateWorkspace(Workspace workspace);
    List<Repository> GetRepositories();
    List<ThirdPartyComponent> GetThirdPartyComponents();
    List<DevProject> GetDevProjects();
    List<ComponentCve> GetComponentCves();
    void SaveTeams(List<Team> teams);
    void SaveWorkspace(List<Workspace> workspaces);
    void SaveRepositories(List<Repository> repositories);
    string UpdateOrInsertRepository(Repository repository);
    bool RemoveRepository(Guid repositoryId);
    void SaveThirdPartyComponents(List<ThirdPartyComponent> components);
    bool InsertComponent(ThirdPartyComponent component);
    void SaveDevProjects(List<DevProject> devProjects);
    bool InsertDevProject(DevProject project);
    int InsertDevProjects(IEnumerable<DevProject> projects);
    void InsertOrUpdateCve(ComponentCve componentCve);
    void ReLoad();
}