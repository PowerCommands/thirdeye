namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;
public class Repository
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public Guid ProjectId { get; set; }
    public Guid RepositoryId { get; set; }
    public Branch? MainBranch { get; set; } = new();
}