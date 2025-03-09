using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Fetch vulnerabilities for your components from NVD database.",
                  disableProxyOutput: true,
                             options: "request-nvd",
                             example: "//Fetch vulnerabilities for your components from NVD database.|nvd")]
    public class NvdCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var nvdFetcher = new NvdDataFetcherManager();
            var components = Storage.GetThirdPartyComponents();
            var cve = nvdFetcher.FetchAllCves().Result;
            WriteSuccessLine($"{cve.Count} persisted in database");
            Storage.SaveCveEntries(cve);

            //foreach (var component in components)
            //{
            //    var entries = ObjectStorage.GetCveEntries().Where(cv => cv.ComponentName == component.Name).Select(en => en.CveEntries).FirstOrDefault() ?? [];
            //    var cveEntries = HasOption("request-nvd") ? nvdFetcher.FetchRecentVulnerabilities().Result : entries;
            //    cve.Add(new ComponentCve { CveEntries = cveEntries, ComponentName = component.Name });
            //}
            //var cveAnalyzer = new CveAnalyzeManager();
            //var vulnerabilities = cveAnalyzer.GetVulnerabilities(cve, components);
            //PresentationManager.DisplayCveEntries(vulnerabilities);
            return Ok();
        }
    }
}