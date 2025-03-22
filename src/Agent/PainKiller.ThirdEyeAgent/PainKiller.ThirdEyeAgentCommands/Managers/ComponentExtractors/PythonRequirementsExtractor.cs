using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers.ComponentExtractors;

public class PythonRequirementsExtractor : IComponentExtractor
{
    public bool CanHandle(Item file) => file.Path.EndsWith("requirements.txt", StringComparison.OrdinalIgnoreCase);

    public List<ThirdPartyComponent> ExtractComponents(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) return new();

        var components = new List<ThirdPartyComponent>();
        var lines = file.Content.Split('\n');

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;

            var parts = trimmed.Split(["==", ">=", "<="], StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                components.Add(new ThirdPartyComponent
                {
                    Name = parts[0].Trim(),
                    Version = parts[1].Trim(),
                    Path = file.Path,
                    CommitId = file.CommitId
                });
            }
        }

        return components;
    }
}