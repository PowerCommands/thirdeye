using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class ThirdPartyComponentObjects
{
    public DateTime LastUpdated { get; set; }
    public List<ThirdPartyComponent> Components { get; set; } = [];
}