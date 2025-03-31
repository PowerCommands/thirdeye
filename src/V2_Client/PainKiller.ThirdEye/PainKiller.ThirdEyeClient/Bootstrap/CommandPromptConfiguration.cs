using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.ThirdEyeClient.Bootstrap.Configuration;

namespace PainKiller.ThirdEyeClient.Bootstrap;
public class CommandPromptConfiguration : ApplicationConfiguration
{
    public ThirdEyeConfiguration ThirdEye { get; set; } = new();
}