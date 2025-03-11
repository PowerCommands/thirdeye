using PainKiller.ThirdEyeAgentCommands.Data;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContent;

public class ThirdEyeInfoPanelContent : IInfoPanelContent
{
    public string GetText()
    {
        var objectStorage = ObjectStorageManager.Service;
        var repos = objectStorage.GetRepositories();
        var components = objectStorage.GetThirdPartyComponents();
        var text = $"{repos.Count} repos with a total of {components.Count} distinct components.";
        ShortText = text;
        return text;
    }
    public string? ShortText { get; private set; } = "";
}