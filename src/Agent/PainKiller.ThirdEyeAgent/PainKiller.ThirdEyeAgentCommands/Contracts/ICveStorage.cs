using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface ICveStorage
{
    List<CveEntry> GetCveEntries();
    void AppendEntries(List<CveEntry> cves, int index);
    DateTime LastUpdated { get; }
    int LastIndexedPage { get; }
    int LoadedCveCount { get; }
    void ReLoad();
    const int PAGE_SIZE = 2000;
}