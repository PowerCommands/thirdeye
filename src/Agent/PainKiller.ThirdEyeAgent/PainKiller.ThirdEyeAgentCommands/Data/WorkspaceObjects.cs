using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class WorkspaceObjects : IDataObjects<Workspace>
{
    public DateTime LastUpdated { get; set; }
    public List<Workspace> Items { get; set; } = [];
}