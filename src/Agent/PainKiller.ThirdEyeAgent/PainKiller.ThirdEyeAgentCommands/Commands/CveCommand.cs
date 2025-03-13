using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Find out details about a specific CVE",
                  disableProxyOutput: true,
                           arguments: "!<cve-id>",
                             example: "//Find out details about a specific CVE|cve <cve-id>")]
    public class CveCommand(string identifier, PowerCommandsConfiguration config) : ThirdEyeBaseCommando(identifier, config)
    {
        public override RunResult Run()
        {
            var cveId = Input.SingleArgument;
            var apiKey = Configuration.Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
            var nvdFetcher = new CveFetcherManager(CveStorage, Configuration.ThirdEyeAgent.Nvd, apiKey,this);
            var response = nvdFetcher.FetchCveDetailsAsync(cveId).Result;
            WriteHeadLine(cveId);
            PresentationManager.DisplayCveDetails(response);
            return Ok();
        }
    }
}