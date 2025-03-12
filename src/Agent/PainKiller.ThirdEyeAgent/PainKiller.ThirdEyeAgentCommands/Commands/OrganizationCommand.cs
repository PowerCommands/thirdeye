using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Shows an overview of your organization, based on what you have configured to fetch.",
                  disableProxyOutput: true,
                             example: "//Show hierarchy of your organization|organization")]
    public class OrganizationCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var workspaces = Storage.GetWorkspaces().GetFilteredWorkspaces(Configuration.ThirdEyeAgent.Workspaces);
            var repositories = Storage.GetRepositories();
            var teams = Storage.GetTeams();
            var projects = Storage.GetProjects();
            PresentationManager.DisplayOrganization(Configuration.ThirdEyeAgent.OrganizationName, workspaces, repositories, teams, projects);
            EnableLog();
            return Ok();
        }
    }
}