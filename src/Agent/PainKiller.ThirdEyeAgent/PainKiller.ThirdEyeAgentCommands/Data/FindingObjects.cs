using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class FindingObjects : IDataObjects<Finding>
{
    public DateTime LastUpdated { get; set; }
    public List<Finding> Items { get; set; } = [];
}