namespace PainKiller.ThirdEyeClient.Contracts;

public interface ICveStorageService
{
    List<CveEntry> GetCveEntries();
    void AppendEntries(List<CveEntry> cves, IConsoleWriter writer);
    DateTime LastUpdated { get; }
    int LoadedCveCount { get; }
    string CreateUpdateFile();
    CveUpdateFileInfo GetUpdateInfo();
    bool NeedsUpdate();
    bool Update(IConsoleWriter writer);
    void ReLoad();
}