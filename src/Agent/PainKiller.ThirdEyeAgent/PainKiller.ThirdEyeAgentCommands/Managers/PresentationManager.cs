using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class PresentationManager(IConsoleWriter writer)
{
    public void DisplayRepository(string name, IEnumerable<Project> projects)
    {
        writer.WriteHeadLine($"\n📁 {name}");
        foreach (var project in projects)
        {
            writer.WriteHeadLine($"├── 🈁 {project.Name} {project.Sdk} {project.Language} {project.Framework}");
            foreach (var component in project.Components)
            {
                writer.WriteHeadLine($"│  │   ├── {component.Name} {component.Version}");
            }
        }
    }

    public void DisplayOrganization(string organizationName, List<Workspace> workspaces, List<Repository> repositories, List<Team> teams, List<Project> projects)
    {
        writer.WriteHeadLine($"\n🏠 {organizationName}");
        foreach (var workspace in workspaces)
        {
            if (repositories.All(p => p.WorkspaceId != workspace.Id)) continue;
            writer.WriteHeadLine($"  ├── 📦 {workspace.Name}");
            var projectTeams = teams.Where(t => t.WorkspaceIds.Any(p => p == workspace.Id));
            foreach (var team in projectTeams)
            {
                writer.WriteHeadLine($"  │   ├── 👨‍👩‍👧‍👦 {team.Name.Trim()}");
                writer.WriteHeadLine("  │   ├── Members");
                foreach (var member in team.Members)
                {
                    writer.WriteHeadLine($"  │   │   ├── 👤 {member.Name}");
                }
            }

            var projectRepos = repositories.Where(r => r.WorkspaceId == workspace.Id);
            writer.WriteHeadLine("  │   ├── Repos");
            foreach (var repository in projectRepos)
            {
                writer.WriteHeadLine($"  │   ├── 📁 {repository.Name}");
                var repoProjects = projects.Where(dp => dp.RepositoryId == repository.RepositoryId);
                foreach (var project in repoProjects)
                {
                    writer.WriteHeadLine($"  │   │   ├── 🈁 {project.Name} {project.Sdk} {project.Language} {project.Framework}");
                    foreach (var component in project.Components)
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
    public CveEntry? DisplayVulnerableComponent(ComponentCve component)
    {
        var padLength = component.CveEntries.Where(c => true).Select(cv => cv.Id.Length).Max();
        writer.WriteHeadLine($"├── {component.Name} {component.Version}");
        var textLength = component.CveEntries.Select(c => $"{c.Id.PadRight(padLength)} {c.CvssScore.GetDisplaySeverity()}").Max(t => t.Length);
        var displayTextLength = Console.WindowWidth - textLength -10;
        var list = ListService.ListDialog("Choose a CVE to view details about it.", component.CveEntries.Select(c => $"{c.Id} {c.CvssScore.GetDisplaySeverity()} {c.Description.Truncate(displayTextLength)}").ToList());
        if(list.Count <= 0) return new CveEntry{Id = "-"};
        if (list.Count == 0) return null;
        var selected = component.CveEntries[list.First().Key];
        return selected;
    }
    public void DisplayCveDetails(CveDetailResponse cveDetails)
    {
        if (cveDetails?.vulnerabilities == null || cveDetails.vulnerabilities.Length == 0)
        {
            writer.WriteHeadLine("🔒 No CVEs found.");
            return;
        }

        writer.WriteHeadLine($"🔒 {cveDetails.vulnerabilities.Length} CVEs found");

        foreach (var cve in cveDetails.vulnerabilities)
        {
            var cveId = cve.cve.id ?? "Unknown ID";
            var description = cve.cve.descriptions?.FirstOrDefault()?.value ?? "No description available";

            // Justerad hantering av CVSS-score och severity
            var cvssMetric = cve.cve.metrics?.cvssMetricV2?.FirstOrDefault();
            var cvssScore = cvssMetric?.cvssData?.baseScore.ToString() ?? "N/A";
            var severity = cvssMetric?.baseSeverity ?? "N/A";

            writer.WriteHeadLine($"├── {cveId} {description}");
            writer.WriteHeadLine($"│  ├── CVSS Score: {cvssScore}");
            writer.WriteHeadLine($"│  ├── Severity: {severity}");
            writer.WriteHeadLine($"│  ├── Affected Products:");

            

            // Justerad hantering av affected products
            var affectedProducts = cve.cve.configurations?
                .Where(c => c.nodes != null)
                .SelectMany(c => c.nodes)
                .Where(n => n.cpeMatch != null)
                .SelectMany(n => n.cpeMatch)
                .Where(m => m.vulnerable)
                .Select(m => m.criteria)
                .Distinct()
                .ToList();

            if (affectedProducts == null || affectedProducts.Count == 0)
            {
                writer.WriteHeadLine($"│  │   ├── No affected products listed.");
            }
            else
            {
                foreach (var product in affectedProducts)
                {
                    writer.WriteHeadLine($"│  │   ├── {product}");
                }
            }
            writer.WriteHeadLine($"│  ├── References:");
            foreach (var cveReference in cve.cve.references)
            {
                writer.WriteHeadLine($"│  │   ├── {cveReference.source} {cveReference.url}");
            }
        }
    }
}