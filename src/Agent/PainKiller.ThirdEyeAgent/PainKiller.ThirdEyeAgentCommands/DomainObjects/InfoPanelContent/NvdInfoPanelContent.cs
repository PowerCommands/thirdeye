using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContent;

public class NvdInfoPanelContent() : IInfoPanelContent
{
    public string GetText()
    {
        var storage = CveStorageService.Service;
        var needsUpdateText = storage.NeedsUpdate() ? "⚠️ Needs Update run [nvd] command" : "";
        ShortText = needsUpdateText;
        var metaData = storage.GetUpdateInfo();
        return $"NVD CVEs count: {metaData.CveCount} Last updated: {metaData.Created.ToShortDateString()} {needsUpdateText}";
    }
    public string? ShortText { get; private set; }
}