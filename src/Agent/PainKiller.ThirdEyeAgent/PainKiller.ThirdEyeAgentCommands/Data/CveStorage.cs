using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class CveStorage : ICveStorage
{
    public CveStorage()
    {
        _storagePath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, "nvd");
        if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
    }
    private readonly string _storagePath;
    private static CveObjects _cveObjects = new();
    public List<CveEntry> GetCveEntries() => _cveObjects.Entries;
    public void AppendEntries(List<CveEntry> cves, int index)
    {
        foreach (var cveEntry in cves.Where(cveEntry => _cveObjects.Entries.All(c => c.Id != cveEntry.Id))) _cveObjects.Entries.Add(cveEntry);
        _cveObjects.LastUpdated = DateTime.Now;
        _cveObjects.LastIndexedPage = index;
        StorageService<CveObjects>.Service.StoreObject(_cveObjects, Path.Combine(_storagePath, $"{nameof(Data.CveObjects)}.json"));
    }
    public int LoadedCveCount => _cveObjects.Entries.Count;
    public DateTime LastUpdated => _cveObjects.LastUpdated;
    public int LastIndexedPage => _cveObjects.LastIndexedPage;
    public void ReLoad() => _cveObjects = StorageService<CveObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(Data.CveObjects)}.json"));
}