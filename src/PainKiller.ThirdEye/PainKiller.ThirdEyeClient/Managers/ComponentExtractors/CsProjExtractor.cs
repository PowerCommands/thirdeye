using System.Xml;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Managers.ComponentExtractors;

public class CsProjExtractor : IComponentExtractor
{
    public bool CanHandle(Item file) => file.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);
    public List<ThirdPartyComponent> ExtractComponents(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) 
            return new List<ThirdPartyComponent>();
        var components = new List<ThirdPartyComponent>();
        var xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.LoadXml(file.Content);
            var packageReferences = xmlDoc.GetElementsByTagName("PackageReference");

            foreach (XmlNode node in packageReferences)
            {
                var packageName = node.Attributes?["Include"]?.Value;
                var packageVersion = node.Attributes?["Version"]?.Value;
                if (!string.IsNullOrEmpty(packageName) && !string.IsNullOrEmpty(packageVersion))
                {
                    components.Add(new ThirdPartyComponent{Name = packageName, Version = packageVersion, Path = file.Path, CommitId = file.CommitId});
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
