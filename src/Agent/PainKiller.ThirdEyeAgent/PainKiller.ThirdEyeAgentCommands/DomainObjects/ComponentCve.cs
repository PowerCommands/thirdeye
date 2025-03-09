using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class ComponentCve
{
    public string ComponentName { get; set; } = "";
    public List<CveEntry> CveEntries { get; set; } = [];
}