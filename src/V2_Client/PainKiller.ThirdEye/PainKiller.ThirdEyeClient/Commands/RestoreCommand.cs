﻿using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Data;

namespace PainKiller.ThirdEyeClient.Commands;
[CommandDesign(description: "Restore your current used workspace.",
    arguments: [],
    examples: ["//Restore your current used workspace", "restore"])]
public class RestoreCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        Restore();
        return Ok();
    }

    private void Restore()
    {
        var rootDirectory = Configuration.ThirdEye.BackupPath.GetReplacedPlaceHolderPath();
        if (!Directory.Exists(rootDirectory))
        {
            Writer.WriteLine("There is no stored backups to restore");
            return;
        }

        var softwareSourceFileName = Path.Combine(rootDirectory, $"{nameof(SoftwareObjects)}.data");
        var softwareDestinationFileName = Path.Combine(Configuration.Core.RoamingDirectory, $"{nameof(SoftwareObjects)}.data");

        if (File.Exists(softwareSourceFileName))
        {
            var restoreSoftwareFile = DialogService.YesNoDialog("Do you want to restore the software file?");
            if (restoreSoftwareFile) 
            {
                File.Copy(softwareSourceFileName, softwareDestinationFileName, true);
                Writer.WriteSuccessLine("Software file restored successfully.");
            }
        }

        var backups = Directory.GetDirectories(rootDirectory);
        if (backups.Length == 0)
        {
            Writer.WriteLine("There is no stored backup workspaces to restore");
            return;
        }

        var sourceTargetInfo = new DirectoryInfo(Storage.StoragePath);
        var selectedBackup = DialogService.PathDialog("Enter output path", sourceTargetInfo.FullName);
        if (string.IsNullOrEmpty(selectedBackup)) return;

        var destinationDir = Storage.StoragePath;
        var confirmDeletion = DialogService.YesNoDialog($"This will delete the destination directory {destinationDir} and its content, continue?");
        if (!confirmDeletion) return;

        Directory.Delete(destinationDir, recursive: true);
        IOService.CopyFolder(selectedBackup, destinationDir);
        Writer.WriteSuccessLine($"Backup from {selectedBackup.GetCompressedPath(50)} has been restored to {destinationDir.GetCompressedPath(50)}");
    }
}


    
