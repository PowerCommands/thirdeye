using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Managers.ComponentExtractors;
public class RDescriptionExtractor : IComponentExtractor
{
    public bool CanHandle(Item file) => file.Path.EndsWith("DESCRIPTION", StringComparison.OrdinalIgnoreCase);

    public List<ThirdPartyComponent> ExtractComponents(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) return new();

        var components = new List<ThirdPartyComponent>();
        var lines = file.Content.Split('\n');

        foreach (var line in lines)
        {
            if (line.StartsWith("Imports:") || line.StartsWith("Depends:") || line.StartsWith("Suggests:"))
            {
                var deps = line.Substring(line.IndexOf(":", StringComparison.Ordinal) + 1)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim().Split(' ')[0]);

                foreach (var dep in deps)
                {
                    components.Add(new ThirdPartyComponent
                    {
                        Name = dep,
                        Version = "", 
                        Path = file.Path,
                        CommitId = file.CommitId
                    });
                }
            }
        }
        return components;
    }
}