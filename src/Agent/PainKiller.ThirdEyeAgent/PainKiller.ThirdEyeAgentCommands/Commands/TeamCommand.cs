namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle teams",
                  disableProxyOutput: true,
                             example: "//List all teams|team")]
    public class TeamCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var teams = DbManager.GetTeams(); 
            var (key, _) = ListService.FilteredListDialog("Choose Team", teams.Select(p => $"{p.Name,-50} {p.Id}").ToList()).First();
            var selectedTeam = teams[key];
            WriteSuccess(selectedTeam.Name);
            return Ok();
        }
    }
}