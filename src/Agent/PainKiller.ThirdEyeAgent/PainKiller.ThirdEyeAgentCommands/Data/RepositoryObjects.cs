using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class RepositoryObjects : IDataObjects<Repository>
{
    public DateTime LastUpdated { get; set; }
    public List<Repository> Items { get; set; } = [];
}