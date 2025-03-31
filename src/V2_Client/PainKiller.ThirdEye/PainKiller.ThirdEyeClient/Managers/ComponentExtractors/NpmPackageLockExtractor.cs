using System.Text.Json;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Managers.ComponentExtractors;
public class NpmPackageLockExtractor : IComponentExtractor
{
    public bool CanHandle(Item file) => file.Path.EndsWith("package-lock.json", StringComparison.OrdinalIgnoreCase);

    public List<ThirdPartyComponent> ExtractComponents(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) return new();

        var components = new List<ThirdPartyComponent>();
        try
        {
            var json = JsonDocument.Parse(file.Content);
            if (json.RootElement.TryGetProperty("dependencies", out var deps))
            {
                foreach (var dep in deps.EnumerateObject())
                {
                    var version = dep.Value.GetProperty("version").GetString();
                    if (!string.IsNullOrEmpty(version))
                    {
                        components.Add(new ThirdPartyComponent
                        {
                            Name = dep.Name,
                            Version = version,
                            Path = file.Path,
                            CommitId = file.CommitId
                        });
                    }
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