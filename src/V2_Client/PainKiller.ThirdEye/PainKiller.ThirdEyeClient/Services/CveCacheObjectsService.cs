using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ThirdEyeClient.Bootstrap;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeClient.Services;
public class CveCacheObjectsService : ICveCacheObjectsService
{
    private readonly string _storagePath;
    private static Lazy<ICveCacheObjectsService>? _lazy;
    public static void Initialize(CommandPromptConfiguration configuration)
    {
        if (_lazy != null) return;
        _lazy = new Lazy<ICveCacheObjectsService>(() => new CveCacheObjectsService(configuration));
    }
    public static ICveCacheObjectsService Service
    {
        get
        {
            if (_lazy == null)
            {
                throw new InvalidOperationException($"{nameof(CveCacheObjectsService)} has not been initialized yet.");
            }
            return _lazy.Value;
        }
    }
    public CveCacheObjectsService(CommandPromptConfiguration configuration)
    {
        _storagePath = Path.Combine(configuration.Core.Modules.Storage.ApplicationDataFolder.GetReplacedPlaceHolderPath(), "nvd");
        if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
    }

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