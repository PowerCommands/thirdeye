using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class CveAnalyzeManager(IConsoleWriter writer)
{
    public List<ComponentCve> GetVulnerabilities(List<CveEntry> cveEntries, List<ThirdPartyComponent> components, CvssSeverity threshold)
    {
        var vulnerableComponents = new List<ComponentCve>();
        foreach (var component in components)
        {
            writer.WriteCodeExample("Analyze", $"{component.Name} {component.Version}");
            Console.CursorTop -= 1;

            var entries = cveEntries.Where(cv => cv.IsAffectedProduct(component.Name, component.Version)).ToList();
            if (entries.Count > 0)
            {
                var thresholdEntries = entries.Where(e => e.CvssScore.IsEqualOrHigher(threshold)).ToList();
                if(thresholdEntries.Count == 0) continue;
                var componentCve = new ComponentCve { Name = component.Name, Version = component.Version, CveEntries = thresholdEntries };
                vulnerableComponents.Add(componentCve);
            }
        }
        return vulnerableComponents;
    }
    public List<ComponentCve> GetVulnerabilities(List<CveEntry> cveEntries, List<Software> softwareItems, CvssSeverity threshold)
    {
        var vulnerableComponents = new List<ComponentCve>();
        foreach (var software in softwareItems)
        {
            writer.WriteCodeExample("Analyze", $"{software.Name} {software.Version}");
            Console.CursorTop -= 1;

            var entries = cveEntries.Where(cv => cv.IsAffectedProduct(software.Name, software.Version)).ToList();
            if (entries.Count > 0)
            {
                var thresholdEntries = entries.Where(e => e.CvssScore.IsEqualOrHigher(threshold)).ToList();
                if(thresholdEntries.Count == 0) continue;
                var componentCve = new ComponentCve { Name = software.Name, Version = software.Version, CveEntries = thresholdEntries };
                vulnerableComponents.Add(componentCve);
            }
        }
        return vulnerableComponents;
    }
}