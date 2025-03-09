using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class TeamObjects 
{
    public DateTime LastUpdated { get; set; }
    public List<Team> Teams { get; set; } = [];
}