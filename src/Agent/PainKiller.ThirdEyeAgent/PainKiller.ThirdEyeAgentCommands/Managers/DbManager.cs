using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;
public class DbManager
{
    private readonly string _storagePath;
    private readonly TeamObjects _teamObjects;
    private readonly ProjectObjects _projectObjects;
    private readonly RepositoryObjects _repositoryObjects;
    public DbManager(string host)
    {
        _storagePath = Path.Combine(ConfigurationGlobals.ApplicationDataFolder, host.Replace("https://","").Replace("http://","").Replace("/",""));
        if(!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
        _teamObjects = StorageService<TeamObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(TeamObjects)}.json"));
        _projectObjects = StorageService<ProjectObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(ProjectObjects)}.json"));
        _repositoryObjects = StorageService<RepositoryObjects>.Service.GetObject(Path.Combine(_storagePath, $"{nameof(RepositoryObjects)}.json"));
    }
    public List<Team> GetTeams() => _teamObjects.Teams;
    public List<Project> GetProjects() => _projectObjects.Projects;
    public List<Repository> GetRepositories() => _repositoryObjects.Repositories;
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

}