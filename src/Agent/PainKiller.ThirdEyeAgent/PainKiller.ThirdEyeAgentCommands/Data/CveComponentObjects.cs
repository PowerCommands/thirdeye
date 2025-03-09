using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class CveComponentObjects
{
    public DateTime LastUpdated { get; set; }
    public List<ComponentCve> ComponentCve { get; set; } = [];
}