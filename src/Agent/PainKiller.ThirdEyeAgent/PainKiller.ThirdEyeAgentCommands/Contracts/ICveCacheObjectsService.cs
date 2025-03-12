using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;
namespace PainKiller.ThirdEyeAgentCommands.Contracts;
public interface ICveCacheObjectsService
{
    List<CveDetailResponse> CveDetails { get; set; }
    bool Store(CveDetailResponse detail);
    CveDetailResponse? Get(string cveId);
}