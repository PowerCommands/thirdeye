using PainKiller.CommandPrompt.CoreLib.Configuration.Services;
using PainKiller.ThirdEyeClient.Configuration;

namespace PainKiller.ThirdEyeClient.BaseClasses;

public class StorageBase
{
    protected StorageBase()
    {
        var config = ConfigurationService.Service.Get<CommandPromptConfiguration>().Configuration;
        CorePath = Path.Combine(config.Core.RoamingDirectory, config.ThirdEye.Host.Replace("https://", "").Replace("http://", "").Replace("/", "").Replace("\\", "_").Replace(":", ""));
        NvdUpdatePath = config.ThirdEye.Nvd.PathToUpdates.GetReplacedPlaceHolderPath();
        BackupPath = config.ThirdEye.BackupPath.GetReplacedPlaceHolderPath();
        NvdPath = config.ThirdEye.Nvd.Path.GetReplacedPlaceHolderPath();
        Host = config.ThirdEye.Host;
    }
    protected string Host { get; private set; }
    protected string CorePath { get; private set; }
    protected string NvdUpdatePath { get; private set; }
    protected string NvdPath { get; private set; }
    protected string BackupPath { get; private set; }

}