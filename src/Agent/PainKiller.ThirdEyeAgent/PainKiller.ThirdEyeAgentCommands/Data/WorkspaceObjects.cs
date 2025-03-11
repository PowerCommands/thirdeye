using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class WorkspaceObjects
{
    public DateTime LastUpdated { get; set; }
    public List<Workspace> Workspaces { get; set; } = [];
}