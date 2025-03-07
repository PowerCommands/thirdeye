namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class Analyze
{
    public List<DevProject> DevProjects { get; set; } = [];
    public List<ThirdPartyComponent> ThirdPartyComponents { get; set; } = [];
}