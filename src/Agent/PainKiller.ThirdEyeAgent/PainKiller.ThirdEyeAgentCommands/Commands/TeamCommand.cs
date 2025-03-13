using PainKiller.ThirdEyeAgentCommands.BaseClasses;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle teams",
                  disableProxyOutput: true,
                             example: "//List all teams|team")]
    public class TeamCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var teams = Storage.GetTeams(); 
            var (key, _) = ListService.ListDialog("Choose Team", teams.Select(p => $"{p.Name,-50} {p.Id}").ToList()).First();
            var selectedTeam = teams[key];
            
            PresentationManager.DisplayTeam(selectedTeam);

            
            //var workspaces = FilterService.Service.GetWorkspaces(Configuration.ThirdEyeAgent.Workspaces.ToList(), selectedTeam).ToList();
            //var selectedWorkspaces = ListService.ListDialog("Chose Workspace", workspaces.Select(w => w.Name).ToList());
            //if (selectedWorkspaces.Count <= 0) return Ok();
            //var selectedWorkspace = workspaces[selectedWorkspaces.First().Key];
            //var doAnalyze = true;
            
            //while (doAnalyze)
            //{
            //    Analyse(selectedWorkspace);
            //    doAnalyze =DialogService.YesNoDialog("Chose another repo (y) or quit? (not y)");
            //}

            return Ok();
        }
    }
}