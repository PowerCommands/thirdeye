using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Contracts;

public interface IObjectStorageService
{
    string StoragePath { get; }
    List<Team> GetTeams();
    List<Workspace> GetWorkspaces();
    List<Repository> GetRepositories();
    List<ThirdPartyComponent> GetThirdPartyComponents();
    List<Project> GetProjects();
    List<ComponentCve> GetComponentCves();
    List<Finding> GetFindings();
    bool RemoveWorkspace(Guid workspaceId);
    void InsertOrUpdateWorkspace(Workspace workspace);
    void InsertFinding(Finding finding);
    void SaveTeams(List<Team> teams);
    void SaveWorkspace(List<Workspace> workspaces);
    void SaveRepositories(List<Repository> repositories);
    string InsertOrUpdateRepository(Repository repository);
    bool RemoveRepository(Guid repositoryId);
    bool RemoveFinding(string findingId);
    void SaveThirdPartyComponents(List<ThirdPartyComponent> components);
    bool InsertComponent(ThirdPartyComponent component);
    void SaveProjects(List<Project> projects);
    bool InsertProject(Project project);
    int InsertProjects(IEnumerable<Project> projects);
    void InsertOrUpdateCve(ComponentCve componentCve);
    void InsertOrUpdateFinding(Finding finding);
    void ReLoad();
}