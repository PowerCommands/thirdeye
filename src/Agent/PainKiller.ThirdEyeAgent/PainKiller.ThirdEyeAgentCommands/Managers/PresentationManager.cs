﻿using PainKiller.PowerCommands.Shared.Extensions;
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
    public List<ComponentCve> DisplayVulnerableComponents(List<ComponentCve> cve)
    {
        
        var padLength = cve.Where(c => c.CveEntries != null).Select(c => c.CveEntries.Where(e => e.Id != null).Select(e => e.Id.Length).DefaultIfEmpty(0).Max()).DefaultIfEmpty(0).Max();
        
        var filter = "";
        var allComponents = cve;
        List<ComponentCve> filteredComponents;
        var inputBuffer = filter;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("➡ Type to filter results, press ENTER to select, BACKSPACE to delete, ESC to exit:");
            Console.Title = inputBuffer;
            filteredComponents = allComponents.Where(c => c.Name.ToLower().Contains(inputBuffer)).ToList();
            
            if (filteredComponents.Count == 0) Console.WriteLine("No matching result... (Press ESC to exit)");
            else
            {
                writer.WriteHeadLine("\n🔒 Vulnerabilities");
                foreach (var componentCve in filteredComponents)
                {
                    writer.WriteHeadLine($"├── {componentCve.Name} {componentCve.Version}");
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
            Console.Write("\nPress enter to continue with all matching items. ");
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape) return filteredComponents;
            if (key.Key == ConsoleKey.Enter && filteredComponents.Count > 0) break;
            if (key.Key == ConsoleKey.Backspace)
            {
                if (inputBuffer.Length > 0) inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1);
            }
            else if (!char.IsControl(key.KeyChar))
            {
                inputBuffer += key.KeyChar;
            }
        }
        return filteredComponents;
    }
}