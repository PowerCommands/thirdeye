using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class CveObjects
{
    public int LastIndexedPage { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<CveEntry> Entries { get; set; } = [];
}