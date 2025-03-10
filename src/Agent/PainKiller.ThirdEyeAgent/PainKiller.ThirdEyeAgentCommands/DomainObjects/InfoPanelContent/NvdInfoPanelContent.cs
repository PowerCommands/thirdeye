using PainKiller.ThirdEyeAgentCommands.Data;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContent;

public class NvdInfoPanelContent() : IInfoPanelContent
{
    public string GetText()
    {
        var storage = CveStorageService.Service;
        storage.ReLoad();
        var updateInfo = storage.NeedsUpdate() ? "⚠️ Needs Update run [nvd] command" : "";
        ShortText = updateInfo;
        return $"Last updated: {storage.LastUpdated} Loaded NVD CVEs: {storage.LoadedCveCount} {updateInfo}";
    }
    public string? ShortText { get; private set; }
}