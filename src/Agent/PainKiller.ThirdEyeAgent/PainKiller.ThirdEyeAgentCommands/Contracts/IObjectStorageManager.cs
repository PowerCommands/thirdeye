using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IObjectStorageManager
{
    List<Team> GetTeams();
    List<Project> GetProjects();
    bool RemoveProject(Guid projectId);
    void InsertOrUpdateProject(Project project);
    List<Repository> GetRepositories();
    List<ThirdPartyComponent> GetThirdPartyComponents();
    List<DevProject> GetDevProjects();
    void SaveTeams(List<Team> teams);
    void SaveProjects(List<Project> projects);
    void SaveRepositories(List<Repository> repositories);
    string UpdateOrInsertRepository(Repository repository);
    bool RemoveRepository(Guid repositoryId);
    void SaveThirdPartyComponents(List<ThirdPartyComponent> components);
    bool InsertComponent(ThirdPartyComponent component);
    void SaveDevProjects(List<DevProject> devProjects);
    bool InsertDevProject(DevProject project);
    int InsertDevProjects(IEnumerable<DevProject> projects);
    void ReLoad();
}