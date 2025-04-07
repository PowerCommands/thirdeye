using PainKiller.CommandPrompt.CoreLib.Modules.InfoPanelModule.Contracts;
using PainKiller.ThirdEyeClient.Configuration;
using PainKiller.ThirdEyeClient.Services;

namespace PainKiller.ThirdEyeClient.DomainObjects;

public class ThirdEyeInfoPanelContent(ThirdEyeConfiguration configuration) : IInfoPanelContent
{
    public string GetText()
    {
        var objectStorage = ObjectStorageService.Service;
        var repos = objectStorage.GetRepositories();
        var components = objectStorage.GetThirdPartyComponents();

        var host = configuration.Host;
        var storage = CveStorageService.Service;
        var needsUpdateText = storage.NeedsUpdate() ? "⚠️ Needs Update run [nvd] command" : "";
        var metaData = storage.GetUpdateInfo();
        var nvdText = $"NVD CVEs count: {metaData.CveCount} Last updated: {metaData.Created.ToShortDateString()} {needsUpdateText}";

        var text = $"Host: {host} {repos.Count} repos with a total of {components.Count} distinct components.\n{nvdText}";
        return text;
    }
    
}