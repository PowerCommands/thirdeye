namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class MitigationLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Created { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = string.Empty;
    public string FindingId { get; set; } = "";
    public string ProjectName { get; set; } = "";
}