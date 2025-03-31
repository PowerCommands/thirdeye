namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;

public class CoreConfiguration
{
    public string Name { get; set; } = "Command Prompt";
    public string Version { get; set; } = "1.0";
    public string Prompt { get; set; } = "pcm>";
    public string DefaultCommand { get; set; } = "";
    public bool ShowLogo { get; set; } = true;
    public List<string> Suggestions { get; set; } = [];
    public string RoamingDirectoryName { get; set; } = $"{nameof(CommandPrompt)}";
    public ModulesConfiguration Modules { get; set; } = new();
}