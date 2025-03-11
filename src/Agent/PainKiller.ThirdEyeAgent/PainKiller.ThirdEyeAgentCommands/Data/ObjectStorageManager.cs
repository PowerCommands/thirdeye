using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;
public class ObjectStorageManager : IObjectStorageManager
{
    private readonly string _storagePath;
    private TeamObjects _teamObjects;
    private ProjectObjects _projectObjects;
    private RepositoryObjects _repositoryObjects;
    private ThirdPartyComponentObjects _thirdPartyComponentObjects;
    private DevProjectObjects _devProjectObjects;
    private readonly CveComponentObjects _cveComponentObjects;

    private static Lazy<IObjectStorageManager>? _lazy;
    public static void Initialize(string host)
    {
        if (_lazy != null) return;
        _lazy = new Lazy<IObjectStorageManager>(() => new ObjectStorageManager(host));
    }
    public static IObjectStorageManager Service
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
    private ObjectStorageManager(string host)
    {
        _storagePath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, host.Replace("https://","").Replace("http://","").Replace("/",""));
        if(!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
        _teamObjects = StorageService<TeamObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(TeamObjects)}.json"));
        _projectObjects = StorageService<ProjectObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(ProjectObjects)}.json"));
        _repositoryObjects = StorageService<RepositoryObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
        _thirdPartyComponentObjects = StorageService<ThirdPartyComponentObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(ThirdPartyComponentObjects)}.json"));
        _devProjectObjects = StorageService<DevProjectObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(DevProjectObjects)}.json"));
        _cveComponentObjects = StorageService<CveComponentObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(CveComponentObjects)}.json"));
    }
    public List<Team> GetTeams() => _teamObjects.Teams;
    public List<Workspace> GetProjects() => _projectObjects.Projects;
    public List<Repository> GetRepositories() => _repositoryObjects.Repositories;
    public List<ThirdPartyComponent> GetThirdPartyComponents() => _thirdPartyComponentObjects.Components;
    public List<DevProject> GetDevProjects() => _devProjectObjects.DevProjects;
    public List<ComponentCve> GetComponentCves() => _cveComponentObjects.ComponentCve;
    public void SaveTeams(List<Team> teams)
    {
        _teamObjects.Teams = teams;
        _teamObjects.LastUpdated = DateTime.Now;
        StorageService<TeamObjects>.Service.StoreObject(_teamObjects, Path.Combine(_storagePath, $"{nameof(TeamObjects)}.json"));
    }
    public void SaveProjects(List<Workspace> projects)
    {
        _projectObjects.Projects = projects;
        _projectObjects.LastUpdated = DateTime.Now;
        StorageService<ProjectObjects>.Service.StoreObject(_projectObjects, Path.Combine(_storagePath, $"{nameof(ProjectObjects)}.json"));
    }
    public void InsertOrUpdateProject(Workspace project)
    {
        var existing = _projectObjects.Projects.FirstOrDefault(p => p.Id == project.Id);
        if (existing != null)
        {
            _projectObjects.Projects.Remove(existing);
            _projectObjects.Projects.Add(project);
            _projectObjects.LastUpdated = DateTime.Now;
            StorageService<RepositoryObjects>.Service.StoreObject(_repositoryObjects, Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
        }
        _projectObjects.Projects.Add(project);
        _projectObjects.LastUpdated = DateTime.Now;
        StorageService<RepositoryObjects>.Service.StoreObject(_repositoryObjects, Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
    }
    public bool RemoveProject(Guid projectId)
    {
        var existing = _projectObjects.Projects.FirstOrDefault(p => p.Id == projectId);
        if (existing == null) return false;
        _projectObjects.Projects.Remove(existing);
        _projectObjects.LastUpdated = DateTime.Now;
        StorageService<ProjectObjects>.Service.StoreObject(_projectObjects, Path.Combine(_storagePath, $"{nameof(ProjectObjects)}.json"));
        return true;
    }
    public void SaveRepositories(List<Repository> repositories)
    {
        _repositoryObjects.Repositories = repositories;
        _repositoryObjects.LastUpdated = DateTime.Now;
        StorageService<RepositoryObjects>.Service.StoreObject(_repositoryObjects, Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
    }
    public string UpdateOrInsertRepository(Repository repository)
    {
        var existing = _repositoryObjects.Repositories.FirstOrDefault(r => r.RepositoryId == repository.RepositoryId);
        if (existing != null)
        {
            _repositoryObjects.Repositories.Remove(existing);
            existing.Name = repository.Name;
            existing.Url = repository.Url;
            existing.MainBranch = repository.MainBranch;
            existing.WorkspaceId = repository.WorkspaceId;
            _repositoryObjects.Repositories.Add(existing);
            _repositoryObjects.LastUpdated = DateTime.Now;
            StorageService<RepositoryObjects>.Service.StoreObject(_repositoryObjects, Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
            return existing.MainBranch?.CommitId ?? "";
        }
        _repositoryObjects.Repositories.Add(repository);
        _repositoryObjects.LastUpdated = DateTime.Now;
        StorageService<RepositoryObjects>.Service.StoreObject(_repositoryObjects, Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
        return repository.MainBranch?.CommitId ?? "";
    }
    public bool RemoveRepository(Guid repositoryId)
    {
        var existing = _repositoryObjects.Repositories.FirstOrDefault(r => r.RepositoryId == repositoryId);
        if (existing == null) return false;
        _repositoryObjects.Repositories.Remove(existing);
        _repositoryObjects.LastUpdated = DateTime.Now;
        StorageService<RepositoryObjects>.Service.StoreObject(_repositoryObjects, Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
        return true;
    }
    public void SaveThirdPartyComponents(List<ThirdPartyComponent> components)
    {
        _thirdPartyComponentObjects.Components = components;
        _thirdPartyComponentObjects.LastUpdated = DateTime.Now;
        StorageService<ThirdPartyComponentObjects>.Service.StoreObject(_thirdPartyComponentObjects, Path.Combine(_storagePath, $"{nameof(ThirdPartyComponentObjects)}.json"));
    }
    public bool InsertComponent(ThirdPartyComponent component)
    {
        if (_thirdPartyComponentObjects.Components.Any(c => c.CommitId == component.CommitId && c.Name == component.Name && c.Version == component.Version && c.Path == component.Path)) return false;
        _thirdPartyComponentObjects.Components.Add(component);
        _thirdPartyComponentObjects.LastUpdated = DateTime.Now;
        StorageService<ThirdPartyComponentObjects>.Service.StoreObject(_thirdPartyComponentObjects, Path.Combine(_storagePath, $"{nameof(ThirdPartyComponentObjects)}.json"));
        return true;
    }
    public void SaveDevProjects(List<DevProject> devProjects)
    {
        _devProjectObjects.DevProjects = devProjects;
        _devProjectObjects.LastUpdated = DateTime.Now;
        StorageService<DevProjectObjects>.Service.StoreObject(_devProjectObjects, Path.Combine(_storagePath, $"{nameof(DevProjectObjects)}.json"));
    }
    public bool InsertDevProject(DevProject project)
    {
        if (_devProjectObjects.DevProjects.Any(p => p.WorkspaceId == project.WorkspaceId && p.RepositoryId == project.RepositoryId && p.Path == project.Path)) return false;
        _devProjectObjects.DevProjects.Add(project);
        _devProjectObjects.LastUpdated = DateTime.Now;
        StorageService<DevProjectObjects>.Service.StoreObject(_devProjectObjects, Path.Combine(_storagePath, $"{nameof(DevProjectObjects)}.json"));
        return true;
    }
    public int InsertDevProjects(IEnumerable<DevProject> projects)
    {
        var insertedCounter = 0;
        foreach (var project in projects)
        {
            var inserted = InsertDevProject(project);
            if(inserted) insertedCounter++;
        }
        return insertedCounter;
    }
    public void InsertOrUpdateCve(ComponentCve componentCve)
    {
        var existing = _cveComponentObjects.ComponentCve.FirstOrDefault(c => c.Name == componentCve.Name && c.Version == componentCve.Version);
        if (existing != null)
        {
            _cveComponentObjects.ComponentCve.Remove(existing);
            _cveComponentObjects.ComponentCve.Add(componentCve);
            _cveComponentObjects.LastUpdated = DateTime.Now;
            StorageService<CveComponentObjects>.Service.StoreObject(_cveComponentObjects, Path.Combine(_storagePath, $"{nameof(CveComponentObjects)}.json"));
        }
        _cveComponentObjects.ComponentCve.Add(componentCve);
        _cveComponentObjects.LastUpdated = DateTime.Now;
        StorageService<CveComponentObjects>.Service.StoreObject(_cveComponentObjects, Path.Combine(_storagePath, $"{nameof(CveComponentObjects)}.json"));
    }
    public void ReLoad()
    {
        _teamObjects = StorageService<TeamObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(TeamObjects)}.json"));
        _projectObjects = StorageService<ProjectObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(ProjectObjects)}.json"));
        _repositoryObjects = StorageService<RepositoryObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
        _thirdPartyComponentObjects = StorageService<ThirdPartyComponentObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(ThirdPartyComponentObjects)}.json"));
        _devProjectObjects = StorageService<DevProjectObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(DevProjectObjects)}.json"));
    }

}