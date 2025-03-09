using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class RepositoryObjects
{
    public DateTime LastUpdated { get; set; }
    public List<Repository> Repositories { get; set; } = [];
}