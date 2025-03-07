using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Shows an overview of your sourcecode host, based on what you have configured to fetch.",
                  disableProxyOutput: true,
                             example: "//Show hierarchy of your sourcecode host|map")]
    public class MapCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var projects = ObjectStorage.GetProjects().GetFilteredProjects(Configuration.ThirdEyeAgent.Projects);
            var repositories = ObjectStorage.GetRepositories();
            var teams = ObjectStorage.GetTeams();
            var devProjects = ObjectStorage.GetDevProjects();
            PresentationManager.DisplayProjects(projects, repositories, teams, devProjects);
            return Ok();
        }
    }
}