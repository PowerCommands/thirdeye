using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class ProjectObjects : IDataObjects<Project>
{
    public DateTime LastUpdated { get; set; }
    public List<Project> Items { get; set; } = [];
}