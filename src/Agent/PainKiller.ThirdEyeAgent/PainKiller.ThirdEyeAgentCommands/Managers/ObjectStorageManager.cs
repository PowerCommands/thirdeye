using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.Data;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;
public class ObjectStorageManager : IObjectStorageManager
{
    private readonly string _storagePath;
    private TeamObjects _teamObjects;
    private ProjectObjects _projectObjects;
    private RepositoryObjects _repositoryObjects;
    private ThirdPartyComponentObjects _thirdPartyComponentObjects;
    private DevProjectObjects _devProjectObjects;
    public ObjectStorageManager(string host)
    {
        _storagePath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, host.Replace("https://","").Replace("http://","").Replace("/",""));
        if(!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
        _teamObjects = StorageService<TeamObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(TeamObjects)}.json"));
        _projectObjects = StorageService<ProjectObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(ProjectObjects)}.json"));
        _repositoryObjects = StorageService<RepositoryObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
        _thirdPartyComponentObjects = StorageService<ThirdPartyComponentObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(ThirdPartyComponentObjects)}.json"));
        _devProjectObjects = StorageService<DevProjectObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(DevProjectObjects)}.json"));
    }
    public List<Team> GetTeams() => _teamObjects.Teams;
    public List<Project> GetProjects() => _projectObjects.Projects;
    public List<Repository> GetRepositories() => _repositoryObjects.Repositories;
    public List<ThirdPartyComponent> GetThirdPartyComponents() => _thirdPartyComponentObjects.Components;
    public List<DevProject> GetDevProjects() => _devProjectObjects.DevProjects;

    public void SaveTeams(List<Team> teams)
    {
        _teamObjects.Teams = teams;
        StorageService<TeamObjects>.Service.StoreObject(_teamObjects, Path.Combine(_storagePath, $"{nameof(TeamObjects)}.json"));
    }
    public void SaveProjects(List<Project> projects)
    {
        _projectObjects.Projects = projects;
        StorageService<ProjectObjects>.Service.StoreObject(_projectObjects, Path.Combine(_storagePath, $"{nameof(ProjectObjects)}.json"));
    }
    public void SaveRepositories(List<Repository> repositories)
    {
        _repositoryObjects.Repositories = repositories;
        StorageService<RepositoryObjects>.Service.StoreObject(_repositoryObjects, Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
    }
    public void SaveThirdPartyComponents(List<ThirdPartyComponent> components)
    {
        _thirdPartyComponentObjects.Components = components;
        StorageService<ThirdPartyComponentObjects>.Service.StoreObject(_thirdPartyComponentObjects, Path.Combine(_storagePath, $"{nameof(ThirdPartyComponentObjects)}.json"));
    }
    public void SaveDevProjects(List<DevProject> devProjects)
    {
        _devProjectObjects.DevProjects = devProjects;
        StorageService<DevProjectObjects>.Service.StoreObject(_devProjectObjects, Path.Combine(_storagePath, $"{nameof(DevProjectObjects)}.json"));
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