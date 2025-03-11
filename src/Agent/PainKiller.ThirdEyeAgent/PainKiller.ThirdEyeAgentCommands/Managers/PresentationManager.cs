using System.Configuration;
using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class PresentationManager(IConsoleWriter writer)
{
    public void DisplayRepository(string name, IEnumerable<DevProject> projects)
    {
        writer.WriteHeadLine($"\n📁 {name}");
        foreach (var devProject in projects)
        {
            writer.WriteHeadLine($"├── 🈁 {devProject.Name} {devProject.Sdk} {devProject.Language} {devProject.Framework}");
            foreach (var component in devProject.Components)
            {
                writer.WriteHeadLine($"│  │   ├── {component.Name} {component.Version}");
            }
        }
    }

    public void DisplayOrganization(string organizationName, List<Project> projects, List<Repository> repositories, List<Team> teams, List<DevProject> devProjects)
    {
        writer.WriteHeadLine($"\n🏠 {organizationName}");
        foreach (var project in projects)
        {
            if (repositories.All(p => p.ProjectId != project.Id)) continue;
            writer.WriteHeadLine($"  ├── 📦 {project.Name}");
            var projectTeams = teams.Where(t => t.ProjectIds.Any(p => p == project.Id));
            foreach (var team in projectTeams)
            {
                writer.WriteHeadLine($"  │   ├── 👨‍👩‍👧‍👦 {team.Name.Trim()}");
                writer.WriteHeadLine("  │   ├── Members");
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
                    foreach (var component in devProject.Components)
                    {
                        writer.WriteHeadLine($"  │   │   ├────── {component.Name} {component.Version}");
                    }
                }
            }
        }
    }
    public void DisplayVulnerableComponents(List<ComponentCve> cve)
    {
        writer.WriteHeadLine("\n🔒 Vulnerabilities");
        var padLength = cve.Where(c => c.CveEntries != null).Select(c => c.CveEntries.Where(e => e.Id != null).Select(e => e.Id.Length).DefaultIfEmpty(0).Max()).DefaultIfEmpty(0).Max();
        var truncate = 73;
        foreach (var componentCve in cve)
        {
            writer.WriteHeadLine($"├── {componentCve.ComponentName}");
            foreach (var cveEntry in componentCve.CveEntries)
            {
                var displayTextLength = Console.WindowWidth - ($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} ".Length) -10;
                var severity = cveEntry.CvssScore.GetSeverity();
                if (severity == CvssSeverity.Medium) writer.WriteHeadLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
                else if (severity == CvssSeverity.Critical) writer.WriteFailureLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
                else if (severity == CvssSeverity.High) writer.WriteFailureLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
                else writer.WriteLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
            }
        }
    }
}