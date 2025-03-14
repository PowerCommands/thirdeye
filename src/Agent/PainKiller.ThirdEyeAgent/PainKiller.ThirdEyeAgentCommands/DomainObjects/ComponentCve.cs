using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class ComponentCve
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public List<CveEntry> CveEntries { get; set; } = [];
    public bool HasVulnerability => CveEntries.Count > 0;
    public float MaxCveEntry => CveEntries.Max(c => c.CvssScore);
    public int VersionOrder
    {
        get
        {
            var parts = Version.Split('.').Select(p => int.TryParse(p, out var num) ? num : 0).ToArray();
            return parts.Length > 0 ? parts.Aggregate(0, (acc, p) => acc * 1000 + p) : 0;
        }
    }
    public bool IsSoftware { get; set; }
}