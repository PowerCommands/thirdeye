using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class PresentationManager(IConsoleWriter writer)
{
    public void DisplayRepository(string name, IEnumerable<DevProject> projects)
    {
        writer.WriteHeadLine($"\n📁 {name}");
        foreach (var devProject in projects)
        {
            writer.WriteHeadLine($"├── 🈁 {devProject.Name} {devProject.Sdk} {devProject.Language} {devProject.Framework}");
            foreach (var component in devProject.Components) {writer.WriteHeadLine($"│  │   ├── {component.Name} {component.Version}");}
        }
    }
    public void DisplayProjects(List<Project> projects, List<Repository> repositories, List<Team> teams, List<DevProject> devProjects)
    {
        writer.WriteHeadLine("\n🏠 Organisation");
        foreach (var project in projects)
        {
            if(repositories.All(p => p.ProjectId != project.Id)) continue;
            writer.WriteHeadLine($"  ├── 📦 {project.Name}");
            var projectTeams = teams.Where(t => t.ProjectIds.Any(p => p == project.Id));
            foreach (var team in projectTeams)
            {
                writer.WriteHeadLine($"  │   ├── 👨‍👩‍👧‍👦 {team.Name.Trim()}");
                writer.WriteHeadLine( "  │   ├── Members");
                foreach (var member in team.Members)
                {
                    writer.WriteHeadLine($"  │   │   ├── 👤 {member.Name}");
                }
            }
            var projectRepos = repositories.Where(r => r.ProjectId == project.Id);
            writer.WriteHeadLine("  │   ├── Repos");
            foreach (var repository in projectRepos)
            {
                writer.WriteHeadLine($"  │   ├── 📁 {repository.Name}");
                var repoDevProjects = devProjects.Where(dp => dp.RepositoryId == repository.RepositoryId);
                foreach (var devProject in repoDevProjects)
                {
                    writer.WriteHeadLine($"  │   │   ├── 🈁 {devProject.Name} {devProject.Sdk} {devProject.Language} {devProject.Framework}");
                    foreach (var component in devProject.Components) {writer.WriteHeadLine($"  │   │   ├────── {component.Name} {component.Version}");}
                }
                //var repoDevProjects = devProjects.Where(dp => dp.RepositoryId == repository.RepositoryId);
                //DisplayRepository(repository.Name, repoDevProjects);
            }
        }
    }
}