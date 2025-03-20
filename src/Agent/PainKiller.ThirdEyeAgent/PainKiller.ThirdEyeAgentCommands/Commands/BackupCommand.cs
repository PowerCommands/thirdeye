using PainKiller.PowerCommands.Configuration.Extensions;
using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Data;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Backup your current used workspace.",
                  disableProxyOutput: true,
                             example: "//Backup your current used workspace|backup")]
    public class BackupCommand(string identifier, PowerCommandsConfiguration configuration) :ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            Backup();
            return Ok();
        }
        private void Backup()
        {
            var rootDirectory = Configuration.ThirdEyeAgent.BackupPath.GetReplacedPlaceHolderPath();
            if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);

            var softwareSourceFileName = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, $"{nameof(SoftwareObjects)}.data");
            var softwareDestinationFileName = Path.Combine(rootDirectory, $"{nameof(SoftwareObjects)}.data");
            File.Copy(softwareSourceFileName, softwareDestinationFileName, true);
            WriteSuccessLine($"Software file {softwareSourceFileName.GetCompressedPath(50)} backed up to {softwareDestinationFileName.GetCompressedPath(50)}");
            var sourceDir = Storage.StoragePath;
            var sourceDirectoryInfo = new DirectoryInfo(sourceDir);
            var targetDir = Path.Combine(rootDirectory, sourceDirectoryInfo.Name.FormatFileTimestamp());
            IOService.CopyFolder(sourceDir, targetDir);
            WriteSuccessLine($"Directory {sourceDir.GetCompressedPath(50)} has been copied up to {targetDir.GetCompressedPath(50)}");
        }
    }
}