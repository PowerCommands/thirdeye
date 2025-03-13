using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class CveComponentObjects : IDataObjects<ComponentCve>
{
    public DateTime LastUpdated { get; set; }
    public List<ComponentCve> Items { get; set; } = [];
}