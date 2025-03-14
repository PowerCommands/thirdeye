using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;

namespace PainKiller.ThirdEyeAgentCommands.Data;
public class CveComponentObjects : IDataObjects<ComponentCve>
{
    public DateTime LastUpdated { get; set; }
    public List<ComponentCve> Items { get; set; } = [];
    public CvssSeverity Severity { get; set; }
}