using PainKiller.PowerCommands.Configuration.Extensions;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using RunResult = PainKiller.PowerCommands.Shared.DomainObjects.Core.RunResult;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Backup your current used workspace.",
                  disableProxyOutput: true,
                             options: "restore",
                             example: "//Backup your current used workspace|backup")]
    public class BackupCommand(string identifier, PowerCommandsConfiguration configuration) :ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            if (HasOption("restore")) return Restore();
            var rootDirectory = Configuration.ThirdEyeAgent.BackupPath.GetReplacedPlaceHolderPath();
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);

            WriteHeadLine("Select a directory to backup");
            var sourceDir = DialogService.SelectDirectoryDialog(ConfigurationGlobals.ApplicationDataFolder);
            if (string.IsNullOrEmpty(sourceDir)) return Ok();
            var sourceDirectoryInfo = new DirectoryInfo(sourceDir);
            var targetDir = Path.Combine(rootDirectory, sourceDirectoryInfo.Name.FormatFileTimestamp());
            IOService.CopyFolder(sourceDir, targetDir);
            WriteSuccessLine($"Directory {sourceDir} has been copied up to {targetDir}");
            return Ok();
        }

        private RunResult Restore()
        {
            var rootDirectory = Configuration.ThirdEyeAgent.BackupPath.GetReplacedPlaceHolderPath();
            if (!Directory.Exists(rootDirectory))
            {
                WriteLine("There is no stored backups to restore");
                return Ok();
            }
            var backups = Directory.GetDirectories(rootDirectory);
            if (backups.Length == 0)
            {
                WriteLine("There is no stored backups to restore");
                return Ok();
            }
            WriteHeadLine("Select a backup to restore");
            var selectedBackup = DialogService.SelectDirectoryDialog(rootDirectory);
            if (string.IsNullOrEmpty(selectedBackup)) return Ok();

            var destinationDir = ConfigurationGlobals.ApplicationDataFolder;
            var confirmDeletion = DialogService.YesNoDialog($"This will delete the destination directory {destinationDir} and it´s content, continue?");
            if(!confirmDeletion) return Ok();
            IOService.CopyFolder(selectedBackup, destinationDir);
            WriteSuccessLine($"Backup from {selectedBackup} has been restored to {destinationDir}");
            return Ok();
        }
    }
}