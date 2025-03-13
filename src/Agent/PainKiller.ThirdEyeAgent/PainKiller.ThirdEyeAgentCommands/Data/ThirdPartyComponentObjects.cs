using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class ThirdPartyComponentObjects : IDataObjects<ThirdPartyComponent>
{
    public DateTime LastUpdated { get; set; }
    public List<ThirdPartyComponent> Items { get; set; } = [];
}