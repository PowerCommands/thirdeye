﻿namespace PainKiller.ThirdEyeClient.Managers;

public class PresentationManager(IConsoleWriter writer)
{
    public void DisplayFindings(IEnumerable<Finding> findings)
    {
        foreach (var finding in findings)
        {
            writer.WriteHeadLine($"\n{Emo.Warning.Icon()} {finding.Cve.Id} - {finding.Status} {finding.Cve.Description.Truncate(100)}");
            writer.WriteHeadLine($"   {Emo.File.Icon()} {finding.Description}");
            writer.WriteHeadLine($"   {Emo.Clock.Icon()} Created: {finding.Created}");
            writer.WriteHeadLine($"   {Emo.Clock.Icon()} Created: {finding.Created}");
        
            writer.WriteHeadLine($"   {Emo.Search.Icon()} Affected Projects: {finding.AffectedProjects.Count}");
            if (finding.Mitigations.Any())
            {
                writer.WriteHeadLine($"   {Emo.Shield.Icon()} Mitigations: {finding.Mitigations.Count}");

            }
        }
    }
    public void DisplayFinding(Finding finding)
    {
        writer.WriteHeadLine($"\n{Emo.Warning.Icon()} {finding.Cve.Id} - {finding.Status}");
        writer.WriteHeadLine($"   {Emo.File.Icon()} {finding.Description}");
        writer.WriteHeadLine($"   {Emo.Clock.Icon()} Created: {finding.Created}");
        writer.WriteHeadLine($"   {Emo.Clock.Icon()} Updated: {finding.Updated}");
        writer.WriteHeadLine($"   {Emo.Search.Icon()} Affected Projects: {finding.AffectedProjects.Count}");
    
        if (finding.Mitigations.Any())
        {
            writer.WriteHeadLine($"   {Emo.Shield.Icon()} Mitigations:");
            foreach (var mitigation in finding.Mitigations)
            {
                writer.WriteHeadLine($"   ├── {Emo.Success.Icon()} {mitigation.Name}");
                writer.WriteHeadLine($"   │  {Emo.Clock.Icon()} Created: {mitigation.Created}");
                writer.WriteHeadLine($"   │  📝 {mitigation.Description}");
            }
        }
        else
        {
            writer.WriteHeadLine($"   {Emo.Error.Icon()} No mitigations found.");
        }
    }


    public void DisplayRepository(string name, IEnumerable<Project> projects)
    {
        writer.WriteHeadLine($"\n{Emo.Directory.Icon()} {name}");
        foreach (var project in projects)
        {
            writer.WriteHeadLine($"├── 🈁 {project.Name} {project.Sdk} {project.Language} {project.Framework}");
            foreach (var component in project.Components)
            {
                writer.WriteHeadLine($"│  │   ├── {component.Name} {component.Version}");
            }
        }
    }
    public void DisplayTeam(Team team)
    {
        writer.WriteHeadLine($"{Emo.Team.Icon()}{team.Name.Trim()}");
        writer.WriteHeadLine("  │   ├── Members");
        foreach (var member in team.Members) writer.WriteHeadLine($"  │   ├── {Emo.Member.Icon()} {member.Name}");
    }
    public void DisplayProject(Project project, string repositoryName)
    {
        writer.WriteHeadLine($"\n{Emo.Directory.Icon()} {repositoryName}");
        writer.WriteHeadLine($" ├── 🈁 {project.Name} {project.Sdk} {project.Language} {project.Framework}");
        foreach (var component in project.Components)
        {
            writer.WriteHeadLine($" │   ├── {component.Name} {component.Version}");
        }
    }
    public void DisplayOrganization(string organizationName, List<Workspace> workspaces, List<Repository> repositories, List<Team> teams, List<Project> projects, ThirdPartyComponent? filter, bool skipEmpty = false)
    {
        _thirdPartyComponentFilter = filter;
        writer.WriteHeadLine($"\n🏠 {organizationName}");
        foreach (var workspace in workspaces)
        {
            if (repositories.All(p => p.WorkspaceId != workspace.Id)) continue;
            writer.WriteHeadLine($"  ├── {Emo.Package.Icon()} {workspace.Name}");
            var projectTeams = teams.Where(t => t.WorkspaceIds.Any(p => p == workspace.Id));
            foreach (var team in projectTeams)
            {
                writer.WriteHeadLine($"  │   ├── {Emo.Team.Icon()} {team.Name.Trim()}");
                writer.WriteHeadLine("  │   ├── Members");
                foreach (var member in team.Members)
                {
                    writer.WriteHeadLine($"  │   │   ├── {Emo.Member.Icon()} {member.Name}");
                }
            }
            var projectRepos = repositories.Where(r => r.WorkspaceId == workspace.Id).ToList();
            if(projectRepos.Count == 0) continue;
            foreach (var repo in projectRepos)
            {
                writer.WriteHeadLine($"  │   │   ├── {Emo.Repository.Icon()} {repo.Name}");
            }
        }
    }
    public void DisplayComponents(string organizationName, List<Workspace> workspaces, List<Repository> repositories, List<Team> teams, List<Project> projects, ThirdPartyComponent? filter, bool skipEmpty = false)
    {
        _thirdPartyComponentFilter = filter;
        writer.WriteHeadLine($"\n🏠 {organizationName}");
        foreach (var workspace in workspaces)
        {
            if (repositories.All(p => p.WorkspaceId != workspace.Id)) continue;
            writer.WriteHeadLine($"  ├── {Emo.Package.Icon()} {workspace.Name}");
            var projectRepos = repositories.Where(r => r.WorkspaceId == workspace.Id).ToList();
            if(projectRepos.Count == 0) continue;
            foreach (var repo in projectRepos)
            {
                writer.WriteHeadLine($"  │   │   ├── {Emo.Repository.Icon()} {repo.Name}");
            }
        }
    }

    private ThirdPartyComponent? _thirdPartyComponentFilter;
    private void ListFilteredProjectRepos(List<Repository> projectRepos)
    {
        foreach (var repository in projectRepos)
        {
            var repoProjects = ObjectStorageService.Service.GetProjects().Where(dp => dp.RepositoryId == repository.RepositoryId).ToList();
            if (repoProjects.Count == 0) continue;
            writer.WriteHeadLine($"  │   ├── {Emo.Directory.Icon()} {repository.Name}");
            foreach (var project in repoProjects)
            {
                var components = project.Components;
                if (_thirdPartyComponentFilter != null)
                {
                    if(!components.Any(c => c.Name == _thirdPartyComponentFilter.Name && c.Version == _thirdPartyComponentFilter.Version)) continue;
                }
                if(project.Components.Count == 0) continue;
                writer.WriteHeadLine($"  │   │   ├── 🈁 {project.Name} {project.Sdk} {project.Language} {project.Framework}");
                foreach (var component in components)
                {
                    if(_thirdPartyComponentFilter == null) continue;
                    if (component.Name != _thirdPartyComponentFilter.Name || component.Version != _thirdPartyComponentFilter.Version) continue;
                    writer.WriteHeadLine($"  │   │   ├────── {component.Name} {component.Version}");
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
            Console.WriteLine($"{Emo.Right.Icon()} Type to filter results, press ENTER {Emo.Enter.Icon()} to select, BACKSPACE {Emo.Backspace.Icon()}  to delete, ESC {Emo.Escape.Icon()} to exit:");
            Console.Title = inputBuffer;
            filteredComponents = allComponents.Where(c => c.Name.ToLower().Contains(inputBuffer)).ToList();
            var showOnlyVulnerabilityCount = (filteredComponents.Count > 25);
            if (filteredComponents.Count == 0) Console.WriteLine($"No matching result... (Press ESC {Emo.Escape.Icon()} to exit)");
            else
            {
                writer.WriteHeadLine($"\n{Emo.Lock.Icon()} Vulnerabilities");
                foreach (var componentCve in filteredComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder))
                {
                    if (showOnlyVulnerabilityCount)
                    {
                        writer.WriteHeadLine($"├── {componentCve.Name} {componentCve.Version} ({componentCve.CveEntries.Count})");
                    }
                    else
                    {
                        writer.WriteHeadLine($"├── {componentCve.Name} {componentCve.Version} (Top 50 shown)");
                        foreach (var cveEntry in componentCve.CveEntries.OrderByDescending(c => c.CvssScore).Take(50))
                        {
                            var displayTextLength = Console.WindowWidth - ($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} ".Length) - 10;
                            var severity = cveEntry.CvssScore.GetSeverity();
                            if (severity == CvssSeverity.Medium) writer.WriteHeadLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
                            else if (severity == CvssSeverity.Critical) writer.WriteLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
                            else if (severity == CvssSeverity.High) writer.WriteLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
                            else writer.WriteLine($"│  ├── {cveEntry.Id.PadRight(padLength)} {cveEntry.CvssScore.GetDisplaySeverity()} {cveEntry.Description.Truncate(displayTextLength)}");
                        }
                    }
                }
            }
            Console.Write($"\nPress enter {Emo.Enter.Icon()} to continue with all matching items. ");
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
        var list = ListService.ListDialog("Choose a CVE to view details about it.", component.CveEntries.OrderByDescending(cv => cv.CvssScore).ThenByDescending(cv => cv.Severity.GetScoreFromString()).Select(c => $"{c.Id} {c.CvssScore.GetDisplaySeverity()} {c.Description.Truncate(displayTextLength)}").ToList());
        if (list.Count == 0) return null;
        var selected = component.CveEntries[list.First().Key];
        return selected;
    }
    public void DisplayCveDetails(CveDetailResponse cveDetails)
    {
        if (cveDetails?.vulnerabilities == null || cveDetails.vulnerabilities.Length == 0)
        {
            writer.WriteHeadLine($"{Emo.Lock.Icon()} No CVEs found.");
            return;
        }

        writer.WriteHeadLine($"{Emo.Lock.Icon()} {cveDetails.vulnerabilities.Length} CVEs found");

        foreach (var cve in cveDetails.vulnerabilities)
        {
            var cveId = cve.cve.id ?? "Unknown ID";
            var description = cve.cve.descriptions?.FirstOrDefault()?.value ?? "No description available";

            var cvssMetric = cve.cve.metrics?.cvssMetricV2?.FirstOrDefault();
            var cvssScore = cvssMetric?.cvssData?.baseScore.ToString() ?? "N/A";
            var severity = cvssMetric?.baseSeverity ?? "N/A";

            writer.WriteHeadLine($"├── {cveId} {description}");
            writer.WriteHeadLine($"│  ├── CVSS Score: {cvssScore}");
            writer.WriteHeadLine($"│  ├── Severity: {severity}");
            writer.WriteHeadLine($"│  ├── Affected Products:");
            
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

    public void DisplayCveEntries(List<CveEntry> cveEntries)
    {
        Console.Clear();
        if (cveEntries.Count == 0) return;
        var takeCount = Console.WindowHeight - 6;
        Console.WriteLine($"{Emo.Lock.Icon()} {takeCount} CVEs displayed (adjusted to console height)");
        Console.WriteLine($"{"CVE ID",-15} | {"CVSS",-5} | {"Severity",-10} | Description");
        Console.WriteLine(new string('-', 80));

        foreach (var cve in cveEntries.OrderByDescending(cv => cv.CvssScore).Take(takeCount))
        {
            var cveId = string.IsNullOrEmpty(cve.Id) ? "Unknown" : cve.Id;
            var description = string.IsNullOrEmpty(cve.Description) ? "No description" : cve.Description;
            var cvssScore = cve.CvssScore > 0 ? cve.CvssScore.ToString("F1") : "N/A";
            var severity = string.IsNullOrEmpty(cve.Severity) ? "N/A" : cve.Severity;

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = cve.CvssScore >= 9.0 ? ConsoleColor.Red : severity.ToLower() switch
            {
                "high" => ConsoleColor.DarkYellow,
                "medium" => ConsoleColor.Yellow,
                "low" => ConsoleColor.White,
                "info" => ConsoleColor.Cyan,
                _ => ConsoleColor.Gray
            };

            Console.WriteLine($"{cveId,-15} | {cvssScore,-5} | {severity,-10} | {description.Truncate(Console.WindowWidth - 50)}");
            Console.ForegroundColor = originalColor;
        }
    }
}
