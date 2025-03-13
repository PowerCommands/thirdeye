using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;
using PainKiller.ThirdEyeAgentCommands.Enums;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class Finding
{
    public string Id => Cve.Id;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; } = DateTime.Now;
    public FindingStatus Status { get; set; } = FindingStatus.New;
    public List<string> AffectedTeams { get; set; } = [];
    public List<Project> AffectedProjects { get; set; } = [];
    public CveEntry Cve { get; set; } = new();
}