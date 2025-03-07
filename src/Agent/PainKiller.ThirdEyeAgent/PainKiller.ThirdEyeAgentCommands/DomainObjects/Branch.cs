namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class Branch
{
    public string CommitId { get; set; } = "";
    public string Author { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsBaseVersion { get; set; } = true;
}