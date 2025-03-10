using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Update your cve:s from Nantional Vulnerability Database (NVD). \nPlease start with a baseline database file so that you don´t need to download every single CVE.",
                  disableProxyOutput: true,
                             options: "!api-key",
                             example: "//Update your cve:s from the last page you collected.|nvd")]
    public class NvdCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            if (HasOption("api-key"))
            {
                SetupApiKey();
                return Ok();
            }
            var apiKey = configuration.Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
            var nvdFetcher = new NvdDataFetcherManager(CveStorage, configuration.ThirdEyeAgent, apiKey,this);
            var cve = nvdFetcher.FetchAllCves().Result;
            WriteSuccessLine($"{cve.Count} updated in database");
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
            return Ok();
        }
        private void SetupApiKey()
        {
            DisableLog();
            var apiKey = GetOptionValue("api-key");
            Configuration.EncryptSecret(EnvironmentVariableTarget.User, ConfigurationGlobals.NvdApiKeyName, apiKey);
            EnableLog();
            WriteSuccessLine($"\nApiKey has been created, you can now fetch CVEs from NVD.");
        }
    }
}