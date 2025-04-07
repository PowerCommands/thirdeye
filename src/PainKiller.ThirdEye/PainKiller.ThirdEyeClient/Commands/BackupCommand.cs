using PainKiller.ThirdEyeClient.Data;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
using PainKiller.ThirdEyeClient.BaseClasses;

namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign( description: "Backup your current used workspace.",
    examples: ["//Backup your current used workspace", "backup"])]
public class BackupCommand(string identifier) :ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        Backup();
        return Ok();
    }
    private void Backup()
    {
        var backupDirectory = Configuration.ThirdEye.BackupPath.GetReplacedPlaceHolderPath();

        var softwareSourceFileName = Path.Combine(Configuration.Core.RoamingDirectory, $"{nameof(SoftwareObjects)}.data");
        var softwareDestinationFileName = Path.Combine(backupDirectory, $"{nameof(SoftwareObjects)}.data");
        File.Copy(softwareSourceFileName, softwareDestinationFileName, true);
        Writer.WriteSuccessLine($"Software file {softwareSourceFileName.GetCompressedPath(50)} backed up to {softwareDestinationFileName.GetCompressedPath(50)}");
            
        var sourceDir = Storage.StoragePath;
        var sourceDirectoryInfo = new DirectoryInfo(sourceDir);
        var targetDir = Path.Combine(backupDirectory, sourceDirectoryInfo.Name.FormatFileTimestamp());
        IOService.CopyFolder(sourceDir, targetDir);
        Writer.WriteSuccessLine($"Directory {sourceDir.GetCompressedPath(50)} has been copied up to {targetDir.GetCompressedPath(50)}");
    }
}