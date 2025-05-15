using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Managers;

namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign(description: "Find out details about a specific CVE or view the latest ones",
    arguments: ["<cve-id>"],
    examples: ["//View the latest fetched CVE:s", "cve", "//Find out details about a specific CVE", "cve <cve-id>"] )]
public class CveCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var argument = $"{input.Arguments.FirstOrDefault()}".ToLower();
        if (string.IsNullOrEmpty(argument))
        {
            if (CveStorage.LoadedCveCount == 0) CveStorage.ReLoad();
            var latest = CveStorage.GetCveEntries()
                .Where(cv => cv.CvssScore.IsEqualOrHigher(CvssSeverity.Critical))
                .OrderByDescending(cv => cv.FetchedIndex)
                .Take(Configuration.ThirdEye.Nvd.LatestCount)
                .ToList();
            
            Writer.WriteDescription("Latest Critical CVEs", $"Found {latest.Count} critical CVEs.");
            PresentationManager.DisplayCveEntries(latest);
            return Ok();
        }

        var cveId = argument;
        var apiKey = Configuration.Core.Modules.Security.DecryptSecret(Configuration.ThirdEye.Nvd.TokenName);
        var nvdFetcher = new CveFetcherManager(CveStorage, Configuration.ThirdEye.Nvd, apiKey, Writer);
        var response = nvdFetcher.FetchCveDetailsAsync(cveId).Result;

        Writer.WriteHeadLine($"Details for CVE: {cveId}");
        if (response != null) 
        {
            PresentationManager.DisplayCveDetails(response);
        }
        else
        {
            Writer.WriteError($"CVE {cveId} not found or could not be fetched.", nameof(CveCommand));
        }
        return Ok();
    }
}
