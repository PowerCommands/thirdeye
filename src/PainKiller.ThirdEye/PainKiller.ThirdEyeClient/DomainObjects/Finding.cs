using PainKiller.ThirdEyeClient.DomainObjects.Nvd;
using PainKiller.ThirdEyeClient.Enums;

namespace PainKiller.ThirdEyeClient.DomainObjects;

public class Finding
{
    public string Id => Cve.Id;
    public string Description { get; set; } = "";
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;
    public FindingStatus Status { get; set; } = FindingStatus.New;
    public List<Project> AffectedProjects { get; set; } = [];
    protected List<ComponentCve> VulnerableComponents = [];
    public List<MitigationLog> Mitigations{ get; set; } = [];
    public CveEntry Cve { get; set; } = new();
}