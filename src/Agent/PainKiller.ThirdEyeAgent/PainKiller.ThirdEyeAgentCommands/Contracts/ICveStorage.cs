using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface ICveStorage
{
    List<CveEntry> GetCveEntries();
    void AppendEntries(List<CveEntry> cves, int index);
    DateTime LastUpdated { get; }
    int LastIndex { get; }
    int LoadedCveCount { get; }
    void ReLoad();
}