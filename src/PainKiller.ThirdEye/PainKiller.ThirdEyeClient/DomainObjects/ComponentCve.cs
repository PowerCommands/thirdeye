namespace PainKiller.ThirdEyeClient.DomainObjects;

public class ComponentCve
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public List<CveEntry> CveEntries { get; set; } = [];
    public bool HasVulnerability => CveEntries.Count > 0;
    public float MaxCveEntry => CveEntries.Max(c => c.CvssScore);
    public int VersionOrder => this.ToVersionOrder();
    public bool IsSoftware { get; set; }
}