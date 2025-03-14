using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Check for vulnerabilities in your components",
                  disableProxyOutput: true,
                             example: "//Check for vulnerabilities in your components|analyze")]
    public class AnalyzeCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            ConsoleService.Service.Clear();
            WriteHeadLine("Loading CVEs...");
            if(CveStorage.LoadedCveCount == 0) CveStorage.ReLoad();

            var teams = FilterService.Service.GetTeams(Configuration.ThirdEyeAgent.Teams.ToList()).ToList();
            var selectedTeams = ListService.ListDialog("Chose Team", teams.Select(t => t.Name).ToList(), autoSelectIfOnlyOneItem: false);
            if (selectedTeams.Count <= 0) return Ok();
            var selectedTeam = teams[selectedTeams.First().Key];

            var workspaces = FilterService.Service.GetWorkspaces(Configuration.ThirdEyeAgent.Workspaces.ToList(), selectedTeam).ToList();
            var selectedWorkspaces = ListService.ListDialog("Chose Workspace", workspaces.Select(w => w.Name).ToList());
            if (selectedWorkspaces.Count <= 0) return Ok();
            var selectedWorkspace = workspaces[selectedWorkspaces.First().Key];
            var doAnalyze = true;
            
            while (doAnalyze)
            {
                Analyze(selectedWorkspace);
                doAnalyze =DialogService.YesNoDialog("Chose another repo (y) or quit? (not y)");
            }
            return Ok();
        }
        
    }
}