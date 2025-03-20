using PainKiller.PowerCommands.Configuration.Extensions;
using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Data;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Restore your current used workspace.",
                  disableProxyOutput: true,
                             example: "//Restore your current used workspace|restore")]
    public class RestoreCommand(string identifier, PowerCommandsConfiguration configuration) :ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            Restore();
            return Ok();
        }
        private void Restore()
        {
            var rootDirectory = Configuration.ThirdEyeAgent.BackupPath.GetReplacedPlaceHolderPath();
            if (!Directory.Exists(rootDirectory))
            {
                WriteLine("There is no stored backups to restore");
                return;
            }
            var softwareSourceFileName = Path.Combine(rootDirectory, $"{nameof(SoftwareObjects)}.data");
            var softwareDestinationFileName = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{nameof(SoftwareObjects)}.data");
            if (File.Exists(softwareSourceFileName))
            {
                var restoreSoftwareFile = DialogService.YesNoDialog("Do you want to restore the software file?");
                if (restoreSoftwareFile) File.Copy(softwareSourceFileName, softwareDestinationFileName, true);
            }
            var backups = Directory.GetDirectories(rootDirectory);
            if (backups.Length == 0)
            {
                WriteLine("There is no stored backups workspaces to restore");
                return;
            }
            var sourceTargetInfo = new DirectoryInfo(Storage.StoragePath);
            var selectedBackup = DialogService.SelectDirectoryDialog(rootDirectory, sourceTargetInfo.Name);
            if (string.IsNullOrEmpty(selectedBackup)) return;

            var destinationDir = Storage.StoragePath;
            var confirmDeletion = DialogService.YesNoDialog($"This will delete the destination directory {destinationDir} and it´s content, continue?");
            if(!confirmDeletion) return;
            Directory.Delete(destinationDir, recursive: true);
            IOService.CopyFolder(selectedBackup, destinationDir);
            WriteSuccessLine($"Backup from {selectedBackup.GetCompressedPath(50)} has been restored to {destinationDir.GetCompressedPath(50)}");
            return;
        }
    }
}