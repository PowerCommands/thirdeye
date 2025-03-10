using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface ICveStorageService
{
    List<CveEntry> GetCveEntries();
    void AppendEntries(List<CveEntry> cves, IConsoleWriter writer);
    DateTime LastUpdated { get; }
    int LoadedCveCount { get; }
    string PathToUpdates { get; }
    string CreateUpdateFile();
    CveUpdateFileInfo GetUpdateInfo();
    bool NeedsUpdate();
    bool Update(IConsoleWriter writer);
    void ReLoad();
}