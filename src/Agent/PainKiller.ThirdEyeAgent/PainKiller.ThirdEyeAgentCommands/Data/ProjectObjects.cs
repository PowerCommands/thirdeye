using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class ProjectObjects
{
    public DateTime LastUpdated { get; set; }
    public List<Project> Projects { get; set; } = [];
}