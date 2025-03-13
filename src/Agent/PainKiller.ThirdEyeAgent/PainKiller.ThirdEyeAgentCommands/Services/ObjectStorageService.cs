using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.Data;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Services;
public class ObjectStorageService : IObjectStorageService
{
    private readonly string _storagePath;
    
    private readonly ObjectStorageBase<TeamObjects, Team> _teamStorage;
    private readonly ObjectStorageBase<WorkspaceObjects, Workspace> _workspaceStorage;
    private readonly ObjectStorageBase<RepositoryObjects, Repository> _repositoryStorage;
    private readonly ObjectStorageBase<ThirdPartyComponentObjects, ThirdPartyComponent> _componentStorage;
    private readonly ObjectStorageBase<ProjectObjects, Project> _projectStorage;
    private readonly ObjectStorageBase<CveComponentObjects, ComponentCve> _cveStorage;

    private static Lazy<IObjectStorageService>? _lazy;
    public static void Initialize(string host)
    {
        if (_lazy != null) return;
        _lazy = new Lazy<IObjectStorageService>(() => new ObjectStorageService(host));
    }
    public static IObjectStorageService Service
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
    private ObjectStorageService(string host)
    {
        _storagePath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, host.Replace("https://","").Replace("http://","").Replace("/",""));
        if(!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
        _teamStorage = new ObjectStorageBase<TeamObjects, Team>(_storagePath);
        _workspaceStorage = new ObjectStorageBase<WorkspaceObjects, Workspace>(_storagePath);
        _repositoryStorage = new ObjectStorageBase<RepositoryObjects, Repository>(_storagePath);
        _componentStorage = new ObjectStorageBase<ThirdPartyComponentObjects, ThirdPartyComponent>(_storagePath);
        _projectStorage = new ObjectStorageBase<ProjectObjects, Project>(_storagePath);
        _cveStorage = new ObjectStorageBase<CveComponentObjects, ComponentCve>(_storagePath);
    }
    public List<Team> GetTeams() => _teamStorage.GetItems();
    public List<Workspace> GetWorkspaces() => _workspaceStorage.GetItems();
    public List<Repository> GetRepositories() => _repositoryStorage.GetItems();
    public List<ThirdPartyComponent> GetThirdPartyComponents() => _componentStorage.GetItems();
    public List<Project> GetProjects() => _projectStorage.GetItems();
    public List<ComponentCve> GetComponentCves() => _cveStorage.GetItems();
    public void SaveTeams(List<Team> teams) => _teamStorage.SaveItems(teams);
    public void SaveWorkspace(List<Workspace> workspaces) => _workspaceStorage.SaveItems(workspaces);
    public void InsertOrUpdateWorkspace(Workspace workspace) => _workspaceStorage.InsertOrUpdate(workspace, w => w.Id == workspace.Id);
    public bool RemoveWorkspace(Guid workspaceId) => _workspaceStorage.Remove(w => w.Id == workspaceId);
    public void SaveRepositories(List<Repository> repositories) => _repositoryStorage.SaveItems(repositories);
    public string InsertOrUpdateRepository(Repository repository)
    {
        _repositoryStorage.InsertOrUpdate(repository, r => r.RepositoryId == repository.RepositoryId);
        return repository.MainBranch?.CommitId ?? "";
    }
    public bool RemoveRepository(Guid repositoryId) => _repositoryStorage.Remove(r => r.RepositoryId == repositoryId);
    public void SaveThirdPartyComponents(List<ThirdPartyComponent> components) => _componentStorage.SaveItems(components);
    public bool InsertComponent(ThirdPartyComponent component) => _componentStorage.Insert(component, c => c.CommitId == component.CommitId && c.Version == component.Version && c.Path == component.Path);
    public void SaveProjects(List<Project> projects) => _projectStorage.SaveItems(projects);
    public bool InsertProject(Project project) => _projectStorage.Insert(project, p => p.WorkspaceId == project.WorkspaceId && p.RepositoryId == project.RepositoryId && p.Path == project.Path);
    public int InsertProjects(IEnumerable<Project> projects)
    {
        var insertedCounter = 0;
        foreach (var project in projects)
        {
            var inserted = InsertProject(project);
            if(inserted) insertedCounter++;
        }
        return insertedCounter;
    }
    public void InsertOrUpdateCve(ComponentCve componentCve) => _cveStorage.InsertOrUpdate(componentCve, cve => cve.Name == componentCve.Name && cve.Version == componentCve.Version);
    public void ReLoad()
    {
        _teamStorage.ReLoad();
        _workspaceStorage.ReLoad();
        _repositoryStorage.ReLoad();
        _componentStorage.ReLoad();
        _projectStorage.ReLoad();
    }
}