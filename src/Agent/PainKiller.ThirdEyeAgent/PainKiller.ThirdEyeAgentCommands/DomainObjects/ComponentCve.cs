using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class ComponentCve
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public List<CveEntry> CveEntries { get; set; } = [];
    public bool HasVulnerability => CveEntries.Count > 0;
}