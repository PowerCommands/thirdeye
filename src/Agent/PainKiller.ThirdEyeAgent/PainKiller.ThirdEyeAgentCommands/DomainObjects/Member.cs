namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;
public class Member
{ 
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Url{ get; set; } = "";
    public bool IsTeamAdmin { get; set; }
    public List<Guid> TeamIds { get; set; } = [];
}