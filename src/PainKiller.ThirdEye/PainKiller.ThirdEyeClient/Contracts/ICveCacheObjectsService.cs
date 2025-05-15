namespace PainKiller.ThirdEyeClient.Contracts;
public interface ICveCacheObjectsService
{
    List<CveDetailResponse> CveDetails { get; set; }
    bool Store(CveDetailResponse detail);
    CveDetailResponse? Get(string cveId);
}