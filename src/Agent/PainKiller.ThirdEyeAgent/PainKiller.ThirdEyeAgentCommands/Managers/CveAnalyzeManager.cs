using System.Collections.Concurrent;
using PainKiller.ThirdEyeAgentCommands.Data;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class CveAnalyzeManager(IConsoleWriter writer)
{
    public List<ComponentCve> GetVulnerabilities(List<CveEntry> cveEntries, List<ThirdPartyComponent> components, CvssSeverity threshold)
    {
        var cacheFile = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{components.GenerateSignature(threshold)}.json");
        if(File.Exists(cacheFile)) return StorageService<CveComponentObjects>.Service.GetObject(Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{components.GenerateSignature(threshold)}.json")).Items;
        
        var vulnerableComponents = new ConcurrentBag<ComponentCve>();
        var fColor = Console.ForegroundColor;
        var bColor = Console.BackgroundColor;

        Parallel.ForEach(components, component =>
        {
            writer.WriteCodeExample("Analyze", $"{component.Name} {component.Version}");

            var entries = cveEntries.Where(cv => cv.IsAffectedProduct(component.Name, component.Version)).ToList();
            if (entries.Count > 0)
            {
                var thresholdEntries = entries.Where(e => e.CvssScore.IsEqualOrHigher(threshold)).ToList();
                if (thresholdEntries.Count > 0)
                {
                    vulnerableComponents.Add(new ComponentCve 
                    { 
                        Name = component.Name, 
                        Version = component.Version, 
                        CveEntries = thresholdEntries 
                    });
                }
            }
        });
        Console.ForegroundColor = fColor;
        Console.BackgroundColor = bColor;
        var retVal = vulnerableComponents.OrderByDescending(c => c.MaxCveEntry).ThenBy(c => c.VersionOrder).ToList();
        var storeObjects = new CveComponentObjects { Items = retVal, LastUpdated = DateTime.Now };
        StorageService<CveComponentObjects>.Service.StoreObject(storeObjects, Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{components.GenerateSignature(threshold)}.json"));
        return retVal;
    }
    public List<ComponentCve> GetVulnerabilities(string fileName) => StorageService<CveComponentObjects>.Service.GetObject(fileName).Items;
}