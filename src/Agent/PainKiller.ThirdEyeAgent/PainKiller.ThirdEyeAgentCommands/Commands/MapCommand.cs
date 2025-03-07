using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Shows an overview of your sourcecode host.",
                  disableProxyOutput: true,
                             example: "//Show hierarchy of your sourcecode host|map")]
    public class MapCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisplayThreeView();
            return Ok();
        }
        public void DisplayThreeView()
        {
            var projects = ObjectStorage.GetProjects().GetFilteredProjects(Configuration.ThirdEyeAgent.Projects);
            var repositories = ObjectStorage.GetRepositories();
            WriteSuccessLine("\n🏠 Organisation");
            foreach (var project in projects)
            {
                if(repositories.All(p => p.ProjectId != project.Id)) continue;
                WriteSuccessLine($"  ├── 📦 {project.Name}");
                var teams = ObjectStorage.GetTeams().Where(t => t.ProjectIds.Any(p => p == project.Id));
                foreach (var team in teams)
                {
                    WriteSuccessLine($"  │   ├── 👨‍👩‍👧‍👦 {team.Name.Trim()}");
                    WriteSuccessLine( "  │   ├── Members");
                    foreach (var member in team.Members)
                    {
                        WriteSuccessLine($"  │   │   ├── 👤 {member.Name}");
                    }
                }
                var projectRepos = repositories.Where(r => r.ProjectId == project.Id);
                WriteSuccessLine("  │   ├── Repos");
                foreach (var repository in projectRepos)
                {
                    WriteSuccessLine($"  │   ├── 📁 {repository.Name}");
                }
            }
        }
    }
}