using PainKiller.ThirdEyeAgentCommands.Data;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContent;

public class NvdInfoPanelContent() : IInfoPanelContent
{
    public string GetText()
    {
        var storage = CveStorageService.Service;
        var needsUpdateText = storage.NeedsUpdate() ? "⚠️ Needs Update run [nvd] command" : "";
        ShortText = needsUpdateText;
        var metaData = storage.GetUpdateInfo();
        return $"Last updated: {metaData.Created} NVD CVEs count: {metaData.CveCount} {needsUpdateText}";
    }
    public string? ShortText { get; private set; }
}