using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Services;
public class CveCacheObjectsService : StorageBase, ICveCacheObjectsService
{
    private CveCacheObjectsService() { }
    public static ICveCacheObjectsService Service { get; } = new CveCacheObjectsService();
    public List<CveDetailResponse> CveDetails { get; set; } = [];
    public bool Store(CveDetailResponse detail)
    {
        var details = StorageService<List<CveDetailResponse>>.Service.GetObject();
        if(details.Any(d => d.CveId == detail.CveId)) return false;
        details.Add(detail);
        StorageService<List<CveDetailResponse>>.Service.StoreObject(details, Path.Combine(CorePath, nameof(CveDetailResponse)));
        return true;
    }
    public CveDetailResponse? Get(string cveId)
    {
        var details = StorageService<List<CveDetailResponse>>.Service.GetObject(Path.Combine(CorePath, nameof(CveDetailResponse)));
        return details.FirstOrDefault(d => d.CveId == cveId);
    }
}