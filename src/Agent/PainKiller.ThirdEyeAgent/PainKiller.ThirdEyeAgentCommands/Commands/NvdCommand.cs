using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Update your cve:s from Nantional Vulnerability Database (NVD). \nPlease start with a baseline database file so that you don´t need to download every single CVE.",
                  disableProxyOutput: true,
                             example: "//Update your cve:s from the last page you collected.|nvd")]
    public class NvdCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var nvdFetcher = new NvdDataFetcherManager(CveStorage, this);
            var cve = nvdFetcher.FetchAllCves().Result;
            WriteSuccessLine($"{cve.Count} updated in database");
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
            return Ok();
        }
    }
}