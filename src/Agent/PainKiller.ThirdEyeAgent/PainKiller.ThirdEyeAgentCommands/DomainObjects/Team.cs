namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Url { get; set; } = "";
    public List<Member> Members { get; set; } = [];
    public List<Guid> WorkspaceIds { get; set; } = [];
}