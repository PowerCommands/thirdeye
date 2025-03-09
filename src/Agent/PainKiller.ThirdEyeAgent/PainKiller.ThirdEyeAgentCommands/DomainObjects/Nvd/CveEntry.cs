namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

public class CveEntry
{
    public string Id { get; set; }
    public string Description { get; set; }
    public float CvssScore { get; set; }
    public string Severity { get; set; }
    public List<string> AffectedProducts { get; set; }
}