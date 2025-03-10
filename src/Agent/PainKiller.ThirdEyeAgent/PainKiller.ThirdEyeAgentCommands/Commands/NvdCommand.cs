using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Update your cve:s from National Vulnerability Database (NVD). \nPlease start with a baseline database file so that you don´t need to download every single CVE.",
                  disableProxyOutput: true,
                             options: "!api-key|sync",
                             example: "//Update your cve:s from the last page you collected.|nvd")]
    public class NvdCommand(string identifier, PowerCommandsConfiguration config) : ThirdEyeBaseCommando(identifier, config)
    {
        public override RunResult Run()
        {
            if (HasOption("api-key"))
            {
                SetupApiKey();
                return Ok();
            }
            if (HasOption("sync"))
            {
                var apiKey = Configuration.Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
                var nvdFetcher = new CveFetcherManager(CveStorage, Configuration.ThirdEyeAgent, apiKey,this);
                var cve = nvdFetcher.FetchAllCves().Result;
                WriteSuccessLine($"{cve.Count} updated in database");
                WriteSeparatorLine();
                var checkSum = CveStorage.CreateUpdateFile();
                WriteSuccessLine($"A new update file created with Checksum: {checkSum}");
            
                IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
                return Ok();
            }
            CveStorage.Update(this);
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