using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using System.Text.Json;

namespace PainKiller.ThirdEyeAgentCommands.Managers.ComponentExtractors;

public class RenvLockExtractor : IComponentExtractor
{
    public bool CanHandle(Item file) => file.Path.EndsWith("renv.lock", StringComparison.OrdinalIgnoreCase);

    public List<ThirdPartyComponent> ExtractComponents(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) return new();

        var components = new List<ThirdPartyComponent>();

        try
        {
            var json = JsonDocument.Parse(file.Content);
            if (json.RootElement.TryGetProperty("Packages", out var packages))
            {
                foreach (var pkg in packages.EnumerateObject())
                {
                    var version = pkg.Value.GetProperty("Version").GetString();
                    components.Add(new ThirdPartyComponent
                    {
                        Name = pkg.Name,
                        Version = version ?? "",
                        Path = file.Path,
                        CommitId = file.CommitId
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing {file.Path}: {ex.Message}");
        }

        return components;
    }
}