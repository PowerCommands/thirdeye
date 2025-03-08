using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IGitManager
{
    void Connect();
    IEnumerable<Project> GetProjects();
    IEnumerable<Team> GetAllTeams();
    IEnumerable<Repository> GetRepositories(Guid projectId);
    IEnumerable<Item> GetAllFilesInRepository(Guid repositoryId);
    string GetContent(Item item, Guid repositoryId);
}