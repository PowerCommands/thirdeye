using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class SoftwareObjects
{
    public DateTime LastUpdated { get; set; }
    public List<Software> Softwares { get; set; } = [];
}