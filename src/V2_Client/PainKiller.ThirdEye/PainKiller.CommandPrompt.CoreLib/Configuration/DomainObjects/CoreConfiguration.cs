namespace PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
public class CoreConfiguration
{
    private string _roamingDirectoryName = $"{nameof(CommandPrompt)}";
    public string Name { get; set; } = "Command Prompt";
    public string Version { get; set; } = "1.0";
    public string Prompt { get; set; } = "pcm>";
    public string DefaultCommand { get; set; } = "";
    public bool ShowLogo { get; set; } = true;
    public List<string> Suggestions { get; set; } = [];
    public string RoamingDirectory
    {
        get => Path.Combine(ApplicationConfiguration.CoreApplicationDataPath, _roamingDirectoryName);
        set
        {
            var dirPath = Path.Combine(ApplicationConfiguration.CoreApplicationDataPath, _roamingDirectoryName, value);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            _roamingDirectoryName = value;
        }
    }
    public ModulesConfiguration Modules { get; set; } = new();
}