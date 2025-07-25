﻿using System.Collections.Concurrent;
using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ThirdEyeClient.Configuration;
using PainKiller.ThirdEyeClient.Data;

namespace PainKiller.ThirdEyeClient.Managers;

public class CveAnalyzeManager(IConsoleWriter writer, CommandPromptConfiguration configuration)
{
    public List<ComponentCve> GetVulnerabilities(List<CveEntry> cveEntries, List<ThirdPartyComponent> components, CvssSeverity threshold)
    {
        var dataFolder = configuration.Core.RoamingDirectory;
        var cacheFile = Path.Combine(dataFolder, $"{components.GenerateSignature(threshold)}.json");
        if (File.Exists(cacheFile))
        {
            var useCache = DialogService.YesNoDialog("This analyze has been runned before, would you like to use that result?");
            if(useCache) return StorageService<CveComponentObjects>.Service.GetObject(Path.Combine(dataFolder, $"{components.GenerateSignature(threshold)}.json")).Items;
            else File.Delete(cacheFile);
        }
        var vulnerableComponents = new ConcurrentBag<ComponentCve>();
        var fColor = Console.ForegroundColor;
        var bColor = Console.BackgroundColor;

        Parallel.ForEach(components, component =>
        {
            writer.WriteDescription("Analyze", $"{component.Name} {component.Version}");

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
        StorageService<CveComponentObjects>.Service.StoreObject(storeObjects, Path.Combine(dataFolder, $"{components.GenerateSignature(threshold)}.json"));
        return retVal;
    }
    public List<ComponentCve> GetVulnerabilities(string fileName) => StorageService<CveComponentObjects>.Service.GetObject(fileName).Items;
}