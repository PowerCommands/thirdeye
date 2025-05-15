using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ThirdEyeClient.BaseClasses;

namespace PainKiller.ThirdEyeClient.Commands;
[CommandDesign(description: "Handle teams",
    arguments: [],
    examples: ["//List all teams", "team"])]
public class TeamCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var teams = Storage.GetTeams();
        var selected = ListService.ListDialog("Choose Team", teams.Select(p => $"{p.Name,-50} {p.Id}").ToList());

        if (selected.Count == 0) return Ok();
        var (key, _) = selected.First();
        var selectedTeam = teams[key];

        PresentationManager.DisplayTeam(selectedTeam);
        return Ok();
    }
}
  
