using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContens;

public class ThirdEyeInfoPanelContent : IInfoPanelContent
{
    public string GetText()
    {
        var configuration = PowerCommandServices.Service.ExtendedConfiguration;
        var objectStorage = new ObjectStorageManager(configuration.ThirdEyeAgent.Host);
        var repos = objectStorage.GetRepositories();
        var components = objectStorage.GetThirdPartyComponents();
        var text = $"{repos.Count} repos with a total of {components.Count} distinct components.";
        ShortText = text;
        return text;
    }
    public string? ShortText { get; private set; } = "";
}