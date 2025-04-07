using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;

namespace PainKiller.ThirdEyeClient.Configuration;
public class CommandPromptConfiguration : ApplicationConfiguration
{
    public ThirdEyeConfiguration ThirdEye { get; set; } = new();
}