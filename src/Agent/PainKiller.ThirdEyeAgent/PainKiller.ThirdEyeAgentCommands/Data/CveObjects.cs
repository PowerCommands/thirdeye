using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class CveObjects
{
    public DateTime LastUpdated { get; set; }
    public List<CveEntry> Entries { get; set; } = [];
}