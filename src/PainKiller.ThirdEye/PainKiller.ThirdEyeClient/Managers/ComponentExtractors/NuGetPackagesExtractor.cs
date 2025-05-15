using System.Xml;
using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Managers.ComponentExtractors;

public class NuGetPackagesExtractor : IComponentExtractor
{
    public bool CanHandle(Item file) => file.Path.EndsWith("packages.config", StringComparison.OrdinalIgnoreCase);

    public List<ThirdPartyComponent> ExtractComponents(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) 
            return new List<ThirdPartyComponent>();
        var components = new List<ThirdPartyComponent>();
        var xmlDoc = new XmlDocument();

        try
        {
            xmlDoc.LoadXml(file.Content);
            var packageNodes = xmlDoc.GetElementsByTagName("package");

            foreach (XmlNode node in packageNodes)
            {
                var packageName = node.Attributes?["id"]?.Value;
                var packageVersion = node.Attributes?["version"]?.Value;

                if (!string.IsNullOrEmpty(packageName) && !string.IsNullOrEmpty(packageVersion))
                {
                    components.Add(new ThirdPartyComponent { Name = packageName, Version = packageVersion, Path = file.Path, CommitId = file.CommitId });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on parsing file {file.Path}: {ex.Message}");
        }
        return components;
    }
}
