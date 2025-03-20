using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Extensions;
using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Find out details about a specific CVE or view the latest ones",
                  disableProxyOutput: true,
                           arguments: "<cve-id>",
                             example: "//View the latest fetched CVE:s|cve|//Find out details about a specific CVE|cve <cve-id>")]
    public class CveCommand(string identifier, PowerCommandsConfiguration config) : ThirdEyeBaseCommando(identifier, config)
    {
        public override RunResult Run()
        {
            if (string.IsNullOrEmpty(Input.SingleArgument))
            {
                if(CveStorage.LoadedCveCount == 0) CveStorage.ReLoad();
                var latest = CveStorage.GetCveEntries().Where(cv => cv.CvssScore.IsEqualOrHigher(CvssSeverity.Critical)).OrderByDescending(cv => cv.FetchedIndex).Take(Configuration.ThirdEyeAgent.Nvd.LatestCount).ToList();
                PresentationManager.DisplayCveEntries(latest);
                return Ok();
            }
            var cveId = Input.SingleArgument;
            var apiKey = Configuration.Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
            var nvdFetcher = new CveFetcherManager(CveStorage, Configuration.ThirdEyeAgent.Nvd, apiKey,this);
            var response = nvdFetcher.FetchCveDetailsAsync(cveId).Result;
            WriteHeadLine(cveId);
            if(response != null) PresentationManager.DisplayCveDetails(response);
            return Ok();
        }
    }
}