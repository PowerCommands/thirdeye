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
            var projects = ObjectStorage.GetProjects().GetFilteredProjects(configuration.ThirdEyeAgent.Projects);
            var repositories = ObjectStorage.GetRepositories();
            var teams = ObjectStorage.GetTeams();
            var devProjects = ObjectStorage.GetDevProjects();
            PresentationManager.DisplayOrganization(configuration.ThirdEyeAgent.OrganizationName, projects, repositories, teams, devProjects);
            EnableLog();
            return Ok();
        }
    }
}