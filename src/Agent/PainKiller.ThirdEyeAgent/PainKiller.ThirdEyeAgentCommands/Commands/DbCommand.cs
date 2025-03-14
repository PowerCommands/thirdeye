using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Information about the stored Third Eye stored data.",
                  disableProxyOutput: true,
                             example: "//Show db information|db")]
    public class DbCommand(string identifier, PowerCommandsConfiguration configuration) :ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var dir = new DirectoryInfo(ConfigurationGlobals.ApplicationDataFolder);
            WriteHeadLine("📁 App directory");
            foreach (var file in dir.GetFiles())
            {
                WriteLine($"├──📄 {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
            }
            foreach (var hostDirectory in dir.GetDirectories())
            {
                if(hostDirectory.Name == "nvd") continue;
                WriteHeadLine($"├──📁 {hostDirectory.Name}");
                foreach (var file in hostDirectory.GetFiles())
                {
                    WriteLine($"│   ├──📄 {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
                }
            }
            var nvdDir = new DirectoryInfo(Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "nvd"));
            WriteHeadLine("📁 NVD  files");
            foreach (var file in nvdDir.GetFiles())
            {
                WriteLine($"├──📄 {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
            }
            var nvdUpdateDir = new DirectoryInfo(Configuration.ThirdEyeAgent.Nvd.PathToUpdates.Replace(ConfigurationGlobals.RoamingDirectoryPlaceholder, ConfigurationGlobals.ApplicationDataFolder));
            WriteHeadLine("📁 Update  files");
            foreach (var file in nvdUpdateDir.GetFiles())
            {
                WriteLine($"├──📄 {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
            }
            Environment.CurrentDirectory = ConfigurationGlobals.ApplicationDataFolder;
            ShellService.Service.OpenDirectory(ConfigurationGlobals.ApplicationDataFolder);
            return Ok();
        }
    }
}