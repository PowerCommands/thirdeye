using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class TeamObjects : IDataObjects<Team>
{
    public DateTime LastUpdated { get; set; }
    public List<Team> Items { get; set; } = [];
}