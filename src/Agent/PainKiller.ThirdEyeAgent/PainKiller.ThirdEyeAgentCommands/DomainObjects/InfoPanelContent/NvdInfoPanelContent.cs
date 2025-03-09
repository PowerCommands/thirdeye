using PainKiller.ThirdEyeAgentCommands.Contracts;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContent;

public class NvdInfoPanelContent(ICveStorage storage) : IInfoPanelContent
{
    public string GetText()
    {
        ShortText = $"Last updated: {storage.LastUpdated} Loaded NVD CVEs: {storage.LoadedCveCount}";
        return ShortText;
    }
    public string? ShortText { get; private set; }
}