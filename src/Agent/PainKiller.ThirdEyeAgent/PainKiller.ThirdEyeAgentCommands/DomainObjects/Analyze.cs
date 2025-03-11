namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class Analyze
{
    public List<Project> Projects { get; set; } = [];
    public List<ThirdPartyComponent> ThirdPartyComponents { get; set; } = [];
}