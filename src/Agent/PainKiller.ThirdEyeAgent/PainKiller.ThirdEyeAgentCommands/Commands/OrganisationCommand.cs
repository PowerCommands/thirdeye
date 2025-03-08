using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Shows an overview of your organisation, based on what you have configured to fetch.",
                  disableProxyOutput: true,
                             example: "//Show hierarchy of your organisation|organisation")]
    public class OrganisationCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var projects = ObjectStorage.GetProjects().GetFilteredProjects(configuration.ThirdEyeAgent.Projects);
            var repositories = ObjectStorage.GetRepositories();
            var teams = ObjectStorage.GetTeams();
            var devProjects = ObjectStorage.GetDevProjects();
            PresentationManager.DisplayOrganisation(configuration.ThirdEyeAgent.OrganisationName, projects, repositories, teams, devProjects);
            EnableLog();
            return Ok();
        }
    }
}