using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Manage your CVEs from National Vulnerability Database (NVD).",
                  disableProxyOutput: true,
                             options: "!api-key|fetch|push-update-file",
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
            if (HasOption("fetch"))
            {
                var apiKey = Configuration.Secret.DecryptSecret(ConfigurationGlobals.NvdApiKeyName);
                var nvdFetcher = new CveFetcherManager(CveStorage, Configuration.ThirdEyeAgent, apiKey,this);
                var cve = nvdFetcher.FetchAllCvesAsync().Result;
                WriteSuccessLine($"{cve.Count} updated in database");
                WriteSeparatorLine();
                var checkSum = CveStorage.CreateUpdateFile();
                WriteSuccessLine($"A new update file created with Checksum: {checkSum}");
            
                IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
                return Ok();
            }
            if(HasOption("push-update-file")){
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