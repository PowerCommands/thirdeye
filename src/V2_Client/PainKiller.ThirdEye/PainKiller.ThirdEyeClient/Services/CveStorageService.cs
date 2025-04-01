using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ThirdEyeClient.Bootstrap;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.Data;
using PainKiller.ThirdEyeClient.DomainObjects;
using PainKiller.ThirdEyeClient.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeClient.Services;

public class CveStorageService : ICveStorageService
{
    private static Lazy<ICveStorageService>? _lazy;
    public static void Initialize(CommandPromptConfiguration configuration)
    {
        if (_lazy != null) return;
        _lazy = new Lazy<ICveStorageService>(() => new CveStorageService(configuration));
    }

    public static ICveStorageService Service
    {
        get
        {
            if (_lazy == null)
            {
                throw new InvalidOperationException($"{nameof(CveStorageService)} has not been initialized yet.");
            }
            return _lazy.Value;
        }
    }
    private CveStorageService(CommandPromptConfiguration configuration)
    {
        PathToUpdates = configuration.ThirdEye.Nvd.PathToUpdates.GetReplacedPlaceHolderPath();
        _storagePath = Path.Combine(configuration.Core.Modules.Storage.ApplicationDataFolder.GetReplacedPlaceHolderPath(), "nvd");
        if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
    }
    private readonly string _storagePath;
    private readonly string _storageFileName = $"{nameof(CveObjects)}.json";
    private static CveObjects _cveObjects = new();
    public List<CveEntry> GetCveEntries() => _cveObjects.Entries;
    public void AppendEntries(List<CveEntry> cves, IConsoleWriter writer)
    {
        var nextIndex = _cveObjects.Entries.Count;
        foreach (var cveEntry in cves.Where(cveEntry => _cveObjects.Entries.All(c => c.Id != cveEntry.Id)))
        {
            cveEntry.FetchedIndex = nextIndex++;
            _cveObjects.Entries.Add(cveEntry);
        }
        _cveObjects.LastUpdated = DateTime.Now;
        StorageService<CveObjects>.Service.StoreObject(_cveObjects, Path.Combine(_storagePath, _storageFileName));
    }
    public int LoadedCveCount => _cveObjects.Entries.Count;
    public DateTime LastUpdated => _cveObjects.LastUpdated;
    public void ReLoad() => _cveObjects = StorageService<CveObjects>.Service.GetObject(Path.Combine(_storagePath, _storageFileName));
    public string CreateUpdateFile()
    {
        if(_cveObjects.Entries.Count == 0) return "";
        if(!Directory.Exists(PathToUpdates)) Directory.CreateDirectory(PathToUpdates);
        var batchFileName = $"{PathToUpdates}\\cve_baseline.json";
        if(File.Exists(batchFileName)) File.Delete(batchFileName);
        File.Copy(Path.Combine(_storagePath, _storageFileName), batchFileName);
        var checkSum = new FileChecksum(batchFileName);
        var cveUpdateFileInfo = new CveUpdateFileInfo { Checksum = checkSum.Mde5Hash, Created = DateTime.Now, CveCount = _cveObjects.Entries.Count };
        StorageService<CveUpdateFileInfo>.Service.StoreObject(cveUpdateFileInfo, $"{PathToUpdates}\\metadata.json");
        return checkSum.Mde5Hash;
    }
    public string PathToUpdates { get; }
    public CveUpdateFileInfo GetUpdateInfo()
    {
        var metaDataFileName = $"{PathToUpdates}\\metadata.json";
        if(!File.Exists(metaDataFileName)) return new CveUpdateFileInfo();
        var updateInfo = StorageService<CveUpdateFileInfo?>.Service.GetObject(metaDataFileName);
        return updateInfo ?? new CveUpdateFileInfo();
    }

    public bool NeedsUpdate()
    {
        var cveUpdateFileInfo = GetUpdateInfo();
        if(cveUpdateFileInfo.Created == DateTime.MinValue) return true;
        var checkSum = new FileChecksum(Path.Combine(_storagePath, _storageFileName));
        return checkSum.Mde5Hash != cveUpdateFileInfo.Checksum;
    }
    public bool Update(IConsoleWriter writer)
    {
        if (!NeedsUpdate())
        {
            writer.WriteLine("File seems to be identical (same checksum), no update needed.");
            return false;
        }
        var updateFileInfo = GetUpdateInfo();
        var updateFileName = $"{PathToUpdates}\\cve_baseline.json";
        var currentFileName = Path.Combine(_storagePath, _storageFileName);
        var backupFileName = $"{currentFileName}.bckup";
        writer.WriteHeadLine($"\nBegining to update {Path.GetFileName(currentFileName)} with {Path.GetFileName(updateFileName)} checksum: {updateFileInfo.Checksum} created: {updateFileInfo.Created.ToShortDateString()}");
        if (File.Exists(backupFileName)) File.Delete(backupFileName);
        if(File.Exists(currentFileName)) File.Move(currentFileName, backupFileName);
        try
        {
            File.Copy(updateFileName, currentFileName);
            if (File.Exists(currentFileName))
            {
                ReLoad();
                var checkSum = new FileChecksum(currentFileName);
                writer.WriteSuccessLine($"CVE database updated with {_cveObjects.Entries.Count} entries.");
                if(checkSum.Mde5Hash == updateFileInfo.Checksum) writer.WriteSuccessLine($"Checksum verified: {checkSum.Mde5Hash}");
                else writer.WriteError($"Checksum verification failed: {checkSum.Mde5Hash} != {updateFileInfo.Checksum}");
                return true;
            }
            throw new IOException("CVE database file missing after copy operation.");
        }
        catch(Exception ex)
        {
            writer.WriteError($"Failed to update CVE database, original file is restored. {ex.Message}");
            if (File.Exists(currentFileName)) File.Delete(currentFileName);
            File.Move(backupFileName, currentFileName);
            return false;
        }
    }
}