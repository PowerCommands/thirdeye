using PainKiller.PowerCommands.Configuration.Extensions;
using PainKiller.PowerCommands.Shared.Enums;
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
            ConsoleService.Service.Clear();

            ConsoleService.Service.WriteLine(nameof(ConfigCommand),@"░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓███████▓▒░░▒▓███████▓▒░       ░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░ 
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓████████▓▒░▒▓█▓▒░▒▓███████▓▒░░▒▓█▓▒░░▒▓█▓▒░      ░▒▓██████▓▒░  ░▒▓██████▓▒░░▒▓██████▓▒░   
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░         ░▒▓█▓▒░   ░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░         ░▒▓█▓▒░   ░▒▓█▓▒░        
   ░▒▓█▓▒░   ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓███████▓▒░       ░▒▓████████▓▒░  ░▒▓█▓▒░   ░▒▓████████▓▒░ 
                                                                                                        ", ConsoleColor.DarkMagenta);

            var dir = new DirectoryInfo(ConfigurationGlobals.ApplicationDataFolder);
            WriteHeadLine($"{Emo.Directory.Icon()} App directory {dir.GetDirectorySize().GetDisplayFormattedFileSize()}");
            foreach (var file in dir.GetFiles())
            {
                WriteLine($"├──{Emo.File.Icon()} {file.Name}");
            }
            foreach (var hostDirectory in dir.GetDirectories())
            {
                if(hostDirectory.Name == "nvd") continue;
                WriteHeadLine($"├──{Emo.Directory.Icon()} {hostDirectory.Name} {hostDirectory.GetDirectorySize().GetDisplayFormattedFileSize()}");
                foreach (var file in hostDirectory.GetFiles())
                {
                    WriteLine($"│   ├──{Emo.File.Icon()} {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
                }
            }
            var nvdDir = new DirectoryInfo(Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "nvd"));
            WriteHeadLine($"{Emo.Directory.Icon()} NVD  files ({nvdDir.GetDirectorySize().GetDisplayFormattedFileSize()})");
            foreach (var file in nvdDir.GetFiles())
            {
                WriteLine($"├──{Emo.File.Icon()} {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
            }
            var nvdUpdateDir = new DirectoryInfo(Configuration.ThirdEyeAgent.Nvd.PathToUpdates.GetReplacedPlaceHolderPath());
            WriteHeadLine($"{Emo.Directory.Icon()} Update  files ({nvdUpdateDir.GetDirectorySize().GetDisplayFormattedFileSize()})");
            foreach (var file in nvdUpdateDir.GetFiles())
            {
                WriteLine($"├──{Emo.File.Icon()} {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
            }
            
            var backupDir = new DirectoryInfo(Configuration.ThirdEyeAgent.BackupPath.GetReplacedPlaceHolderPath());
            WriteHeadLine($"{Emo.Directory.Icon()} Backup  workspaces ({backupDir.GetDirectorySize().GetDisplayFormattedFileSize()})");
            foreach (var directory in backupDir.GetDirectories())
            {
                WriteLine($"├──{Emo.Directory.Icon()} {directory.Name}");
            }
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();

            var openDir = DialogService.YesNoDialog("Will you like me to open the main data directories for you?");
            if (openDir)
            {
                ShellService.Service.OpenDirectory(Configuration.ThirdEyeAgent.BackupPath.GetReplacedPlaceHolderPath());
                ShellService.Service.OpenDirectory(Configuration.ThirdEyeAgent.Nvd.PathToUpdates.GetReplacedPlaceHolderPath());
                Environment.CurrentDirectory = ConfigurationGlobals.ApplicationDataFolder;
                ShellService.Service.OpenDirectory(ConfigurationGlobals.ApplicationDataFolder);
            }
            return Ok();
        }
    }
}