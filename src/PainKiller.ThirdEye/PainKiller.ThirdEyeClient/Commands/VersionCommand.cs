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
                var list2 = list.ToList();
                for (int i = 0; i < list2.Count; i++)
                {
                    var item = list2[i];
                    if (item.Versions.Count < 2) continue;
                    var prefix = i == selectedIndex ? "> " : "  ";
                    Console.WriteLine($"{prefix}{item.Name} ({item.Versions.Count})");
                }
            },
            (selectedGroup) =>
            {
                if (selectedGroup == null) return;

                if (selectedGroup.Versions.Count == 1)
                {
                    ShowUsage(selectedGroup.Name, selectedGroup.Versions.First());
                    return;
                }
                var versionSelection = ListService.ListDialog($"Choose a version of {selectedGroup.Name}", selectedGroup.Versions.ToList());
                if (versionSelection.Count == 0) return;
                var selectedVersion = selectedGroup.Versions[versionSelection.First().Key];
                ShowUsage(selectedGroup.Name, selectedVersion);
            }
        );
        return Ok();
    }
    private void ShowUsage(string componentName, string version)
    {
        var projects = Storage.GetProjects().Where(p => p.Components.Any(c => c.Name == componentName && c.Version == version)).ToList();
        var repos = Storage.GetRepositories().Where(r => projects.Any(p => p.RepositoryId == r.RepositoryId)).ToList();

        Console.Clear();
        Console.WriteLine($"Component: {componentName}");
        Console.WriteLine($"Version: {version}");
        Console.WriteLine($"\nUsed in {projects.Count} project(s):\n");

        foreach (var project in projects)
        {
            var repo = repos.FirstOrDefault(r => r.RepositoryId == project.RepositoryId);
            var repoName = repo?.Name ?? "Unknown Repo";

            var matchingComponent = project.Components.FirstOrDefault(c => c.Name == componentName && c.Version == version);
            var user = matchingComponent?.UserId;
            var userDisplay = string.IsNullOrWhiteSpace(user) ? "" : $" (User: {user})";

            Console.WriteLine($"- {project.Name} [{repoName}]{userDisplay}");
        }
    }
    private class ComponentVersionGroup
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Versions { get; set; } = new();
    }
}