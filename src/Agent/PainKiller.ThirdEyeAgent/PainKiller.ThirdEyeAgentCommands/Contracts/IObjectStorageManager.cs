using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IObjectStorageManager
{
    List<Team> GetTeams();
    List<Project> GetProjects();
    List<Repository> GetRepositories();
    List<ThirdPartyComponent> GetThirdPartyComponents();
    List<DevProject> GetDevProjects();
    void SaveTeams(List<Team> teams);
    void SaveProjects(List<Project> projects);
    void SaveRepositories(List<Repository> repositories);
    void SaveThirdPartyComponents(List<ThirdPartyComponent> components);
    void SaveDevProjects(List<DevProject> devProjects);
    void ReLoad();
}