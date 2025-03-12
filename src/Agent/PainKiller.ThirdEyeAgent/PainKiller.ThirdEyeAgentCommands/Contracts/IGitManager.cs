using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IGitManager
{
    void Connect();
    IEnumerable<Workspace> GetWorkspaces();
    IEnumerable<Team> GetAllTeams();
    IEnumerable<Repository> GetRepositories(Guid workspaceId);
    IEnumerable<Item> GetAllFilesInRepository(Guid repositoryId);
    //string GetContent(Item item, Guid repositoryId);
}