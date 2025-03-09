using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class CveAnalyzeManager
{
    public List<ComponentCve> GetVulnerabilities(List<ComponentCve> cveEntries, List<ThirdPartyComponent> components)
    {
        var vulnerabilities = new List<ComponentCve>();

        foreach (var cveEntry in cveEntries)
        {
            var matchingComponents = components.Where(c => c.Name == cveEntry.ComponentName);

            foreach (var match in matchingComponents)
            {
                if (cveEntry.CveEntries.Any(c => c.AffectedProducts.Any(p => p.Contains(match.Version))))
                {
                    vulnerabilities.Add(new ComponentCve { ComponentName = match.Name, CveEntries = cveEntry.CveEntries.Where(ce => ce.AffectedProducts.Any(p => p.Contains(match.Version))).ToList() });
                }
            }
        }
        return vulnerabilities;
    }

}