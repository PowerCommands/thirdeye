using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class DevProjectObjects
{
    public DateTime LastUpdated { get; set; }
    public List<DevProject> DevProjects { get; set; } = [];
}