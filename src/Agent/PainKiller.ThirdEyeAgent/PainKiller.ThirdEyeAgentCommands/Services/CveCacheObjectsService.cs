using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Services;
public class CveCacheObjectsService : ICveCacheObjectsService
{
    private readonly string _storagePath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "nvd");
    private static readonly Lazy<ICveCacheObjectsService> Lazy = new(() => new CveCacheObjectsService());
    public static ICveCacheObjectsService Service => Lazy.Value;
    private CveCacheObjectsService(){}

    public List<CveDetailResponse> CveDetails { get; set; } = [];
    public bool Store(CveDetailResponse detail)
    {
        var details = StorageService<List<CveDetailResponse>>.Service.GetObject();
        if(details.Any(d => d.CveId == detail.CveId)) return false;
        details.Add(detail);
        StorageService<List<CveDetailResponse>>.Service.StoreObject(details, Path.Combine(_storagePath, nameof(CveDetailResponse)));
        return true;
    }
    public CveDetailResponse? Get(string cveId)
    {
        var details = StorageService<List<CveDetailResponse>>.Service.GetObject(Path.Combine(_storagePath, nameof(CveDetailResponse)));
        return details.FirstOrDefault(d => d.CveId == cveId);
    }
}