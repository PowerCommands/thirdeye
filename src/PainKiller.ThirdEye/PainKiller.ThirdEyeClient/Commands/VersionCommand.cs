namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign(description: "Show distinct components and number of different versions.",
    examples: ["//List components and version counts", "version"])]
public class VersionCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var filter = $"{input.Arguments.FirstOrDefault()}".ToLower();
        var allComponents = Storage.GetThirdPartyComponents();
        if (!string.IsNullOrWhiteSpace(filter))
            allComponents = allComponents.Where(c => c.Name.ToLower().Contains(filter)).ToList();

        var grouped = allComponents
            .GroupBy(c => c.Name)
            .Select(g => new ComponentVersionGroup
            {
                Name = g.Key,
                Versions = g.Select(c => c.Version).Distinct().OrderBy(v => v).ToList()
            })
            .Where(g => g.Versions.Count > 1)
            .OrderByDescending(g => g.Versions.Count)
            .ThenBy(g => g.Name)
            .ToList();

        InteractiveFilter<ComponentVersionGroup>.Run(
            grouped,
            (component, filterString) => component.Name.ToLower().Contains(filterString.ToLower()),
            (list, selectedIndex) =>
            {
                Console.Clear();
                Console.WriteLine("Choose a component (use arrow keys and Enter):");
                var listExpanded = list.ToList();
                for (int i = 0; i < listExpanded.Count; i++)
                {
                    var item = listExpanded[i];
                    var prefix = i == selectedIndex ? "> " : "  ";
                    Console.WriteLine($"{prefix}{item.Name} ({item.Versions.Count})");
                }
            },
            (selectedGroup) =>
            {
                if (selectedGroup == null) return;

                var versionSelection = ListService.ListDialog(
                    $"Choose the latest (ok) version of {selectedGroup.Name}",
                    selectedGroup.Versions.ToList()
                );

                if (versionSelection.Count == 0) return;

                var selectedVersion = selectedGroup.Versions[versionSelection.First().Key];
                var olderVersions = selectedGroup.Versions.Where(v => string.Compare(v, selectedVersion, StringComparison.OrdinalIgnoreCase) < 0).ToList();

                if (olderVersions.Count == 0)
                {
                    Writer.WriteLine("No older versions found.");
                    return;
                }

                var projects = new List<Project>();
                var repos = new List<Repository>();
                var name = selectedGroup.Name;

                var existingProjectIds = new HashSet<string>();
                var existingRepoIds = new HashSet<Guid>();

                foreach (var oldVersion in olderVersions)
                {
                    var matchedProjects = Storage.GetProjects()
                        .Where(p => p.Components.Any(c => c.Name == name && c.Version == oldVersion) && existingProjectIds.Add(p.Path))
                        .ToList();

                    projects.AddRange(matchedProjects);

                    var matchedRepos = Storage.GetRepositories()
                        .Where(r => matchedProjects.Any(p => p.RepositoryId == r.RepositoryId) && existingRepoIds.Add(r.RepositoryId))
                        .ToList();

                    repos.AddRange(matchedRepos);
                }

                ShowUsage(selectedGroup.Name, projects, repos);
            }
        );
        return Ok();
    }
    private void ShowUsage(string componentName, List<Project> projects, List<Repository> repos)
    {
        Writer.WriteSeparator();
        Writer.WriteLine($"Component: {componentName}");
        Writer.WriteLine($"\nUsed in {projects.Count} project(s):\n");

        foreach (var project in projects)
        {
            var repo = repos.FirstOrDefault(r => r.RepositoryId == project.RepositoryId);
            var repoName = repo?.Name ?? "Unknown Repo";

            var matchingComponent = project.Components.FirstOrDefault(c => c.Name == componentName);
            var user = matchingComponent?.UserId;
            var userDisplay = string.IsNullOrWhiteSpace(user) ? "" : $" (User: {user})";

            Console.WriteLine($"{componentName} {matchingComponent?.Version} - {project.Name} [{repoName}]{userDisplay}");
        }
        var fileName = CreateReport(repos, projects, componentName);
        Writer.WriteSuccessLine($"Successfully write a report to file: {fileName}");
    }
    private class ComponentVersionGroup
    {
        public string Name { get; init; } = string.Empty;
        public List<string> Versions { get; init; } = new();
    }
}