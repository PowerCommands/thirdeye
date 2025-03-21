using PainKiller.ThirdEyeAgentCommands.BaseClasses;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Information about configuration.",
                  disableProxyOutput: true,
                             example: "//Show db information|db")]
    public class ConfigCommand(string identifier, PowerCommandsConfiguration configuration) :ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            ConsoleService.Service.Clear();

            ConsoleService.Service.WriteLine(nameof(ConfigCommand),@"░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓███████▓▒░░▒▓███████▓▒░       ░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░ 
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓████████▓▒░▒▓█▓▒░▒▓███████▓▒░░▒▓█▓▒░░▒▓█▓▒░      ░▒▓██████▓▒░  ░▒▓██████▓▒░░▒▓██████▓▒░   
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░         ░▒▓█▓▒░   ░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░         ░▒▓█▓▒░   ░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓███████▓▒░       ░▒▓████████▓▒░  ░▒▓█▓▒░   ░▒▓████████▓▒░ 
                                                                                                        ", ConsoleColor.DarkMagenta);

            WriteHeadLine("⚙️ Configuration");
            WriteCodeExample($"├── Host: ", Configuration.ThirdEyeAgent.Host);
            WriteCodeExample($"├── Organization: ", Configuration.ThirdEyeAgent.OrganizationName);
            foreach (var team in Configuration.ThirdEyeAgent.Teams) WriteCodeExample($"├──👨 Team", team);
            foreach (var workspace in Configuration.ThirdEyeAgent.Workspaces) WriteCodeExample($"├──📦 Workspace", workspace);
            foreach (var project in Configuration.ThirdEyeAgent.Ignores.Projects) WriteCodeExample($"├──📁 Ignore project", project);
            foreach (var repository in Configuration.ThirdEyeAgent.Ignores.Repositories) WriteCodeExample($"├──📁 Ignore repo", repository);
            
            WriteHeadLine("👾 NVD");
            WriteCodeExample($"├── Url: ", Configuration.ThirdEyeAgent.Nvd.Url);
            WriteCodeExample($"├── Path to updates: ", Configuration.ThirdEyeAgent.Nvd.PathToUpdates);
            WriteCodeExample($"├── Path to backups: ", Configuration.ThirdEyeAgent.BackupPath);
            WriteCodeExample($"├── DelayIntervalSeconds: ", $"{Configuration.ThirdEyeAgent.Nvd.DelayIntervalSeconds}");
            WriteCodeExample($"├── PageSize: ", $"{Configuration.ThirdEyeAgent.Nvd.PageSize}");
            WriteCodeExample($"├── TimeoutSeconds: ", $"{Configuration.ThirdEyeAgent.Nvd.TimeoutSeconds}");

            var authorizationToken = EnvironmentService.Service.GetEnvironmentVariable(ConfigurationGlobals.GetAccessTokenName(false));
            var gitToken = EnvironmentService.Service.GetEnvironmentVariable(ConfigurationGlobals.GetAccessTokenName(true));
            var nvdApiKey = EnvironmentService.Service.GetEnvironmentVariable(ConfigurationGlobals.NvdApiKeyName);
            if(!string.IsNullOrEmpty(authorizationToken)) WriteSuccessLine("Authorization token found: ✅");
            if (!string.IsNullOrEmpty(gitToken)) WriteSuccessLine($"{"Git token found:", -26} ✅");
            if (!string.IsNullOrEmpty(nvdApiKey)) WriteSuccessLine($"{"NVD API key found:", -26} ✅");
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();

            return Ok();
        }
    }
}