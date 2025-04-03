using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Enums;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Bootstrap;
using Spectre.Console;

namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign(description: "Information about the stored Third Eye stored data.",
                 arguments: [],
                  examples: ["//Show db information", "db"])]
public class DbCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override void OnInitialized()
    {
        if (!Directory.Exists(Configuration.Core.RoamingDirectory)) Directory.CreateDirectory(Configuration.Core.RoamingDirectory);
        if (!Directory.Exists(Configuration.ThirdEye.BackupPath)) Directory.CreateDirectory(Configuration.ThirdEye.BackupPath);
        if (!Directory.Exists(Configuration.ThirdEye.Nvd.Path)) Directory.CreateDirectory(Configuration.ThirdEye.Nvd.Path);
        if (!Directory.Exists(Configuration.ThirdEye.Nvd.PathToUpdates)) Directory.CreateDirectory(Configuration.ThirdEye.Nvd.PathToUpdates);
        InfoPanelService.Instance.RegisterContent(new DefaultInfoPanel(new DefaultInfoPanelContent()));
    }

    public override RunResult Run(ICommandLineInput input)
    {
        Writer.Clear();
        Startup.ShowLogo(Configuration.Core);

        var dir = new DirectoryInfo(Configuration.Core.RoamingDirectory);
        Writer.WriteHeadLine($"{Emo.Directory.Icon()} App directory {dir.GetDirectorySize().GetDisplayFormattedFileSize()}");
        foreach (var file in dir.GetFiles())
        {
            Writer.WriteLine($"├──{Emo.File.Icon()} {file.Name}");
        }
        foreach (var hostDirectory in dir.GetDirectories())
        {
            if (hostDirectory.Name == "nvd") continue;
            Writer.WriteHeadLine($"├──{Emo.Directory.Icon()} {hostDirectory.Name} {hostDirectory.GetDirectorySize().GetDisplayFormattedFileSize()}");
            foreach (var file in hostDirectory.GetFiles())
            {
                Writer.WriteLine($"│   ├──{Emo.File.Icon()} {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
            }
        }

        var nvdDir = new DirectoryInfo(Path.Combine(Configuration.Core.RoamingDirectory, "nvd"));
        Writer.WriteHeadLine($"{Emo.Directory.Icon()} NVD files ({nvdDir.GetDirectorySize().GetDisplayFormattedFileSize()})");
        foreach (var file in nvdDir.GetFiles())
        {
            Writer.WriteLine($"├──{Emo.File.Icon()} {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
        }

        var nvdUpdateDir = new DirectoryInfo(Configuration.ThirdEye.Nvd.PathToUpdates.GetReplacedPlaceHolderPath());
        Writer.WriteHeadLine($"{Emo.Directory.Icon()} Update files ({nvdUpdateDir.GetDirectorySize().GetDisplayFormattedFileSize()})");
        foreach (var file in nvdUpdateDir.GetFiles())
        {
            Writer.WriteLine($"├──{Emo.File.Icon()} {file.Name} {file.Length.GetDisplayFormattedFileSize()}");
        }

        var backupDir = new DirectoryInfo(Configuration.ThirdEye.BackupPath.GetReplacedPlaceHolderPath());
        Writer.WriteHeadLine($"{Emo.Directory.Icon()} Backup workspaces ({backupDir.GetDirectorySize().GetDisplayFormattedFileSize()})");
        foreach (var directory in backupDir.GetDirectories())
        {
            Writer.WriteLine($"├──{Emo.Directory.Icon()} {directory.Name}");
        }

        var openDir = DialogService.YesNoDialog("Would you like me to open the main data directories for you?");
        if (openDir)
        {
            ShellService.Default.OpenDirectory(Configuration.ThirdEye.BackupPath.GetReplacedPlaceHolderPath());
            ShellService.Default.OpenDirectory(Configuration.ThirdEye.Nvd.PathToUpdates.GetReplacedPlaceHolderPath());
            Environment.CurrentDirectory = Configuration.Core.RoamingDirectory;
            ShellService.Default.OpenDirectory(Configuration.Core.RoamingDirectory);
        }
        return Ok();
    }
}
