﻿using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.Data;

namespace PainKiller.ThirdEyeClient.Services;
public class ObjectStorageService : StorageBase, IObjectStorageService
{
    private readonly ObjectStorageBase<TeamObjects, Team> _teamStorage;
    private readonly ObjectStorageBase<WorkspaceObjects, Workspace> _workspaceStorage;
    private readonly ObjectStorageBase<RepositoryObjects, Repository> _repositoryStorage;
    private readonly ObjectStorageBase<ThirdPartyComponentObjects, ThirdPartyComponent> _componentStorage;
    private readonly ObjectStorageBase<ProjectObjects, Project> _projectStorage;
    private readonly ObjectStorageBase<CveComponentObjects, ComponentCve> _cveStorage;
    private readonly ObjectStorageBase<FindingObjects, Finding> _findingsStorage;

    public static IObjectStorageService Service { get; } = new ObjectStorageService();
    public string StoragePath { get; private set; }
    private ObjectStorageService()
    {
        StoragePath = CorePath;
        _teamStorage = new ObjectStorageBase<TeamObjects, Team>(StoragePath);
        _workspaceStorage = new ObjectStorageBase<WorkspaceObjects, Workspace>(StoragePath);
        _repositoryStorage = new ObjectStorageBase<RepositoryObjects, Repository>(StoragePath);
        _componentStorage = new ObjectStorageBase<ThirdPartyComponentObjects, ThirdPartyComponent>(StoragePath);
        _projectStorage = new ObjectStorageBase<ProjectObjects, Project>(StoragePath);
        _cveStorage = new ObjectStorageBase<CveComponentObjects, ComponentCve>(StoragePath);
        _findingsStorage = new ObjectStorageBase<FindingObjects, Finding>(StoragePath);
    }
    public List<Team> GetTeams() => _teamStorage.GetItems();
    public List<Workspace> GetWorkspaces() => _workspaceStorage.GetItems();
    public List<Repository> GetRepositories() => _repositoryStorage.GetItems();
    public List<ThirdPartyComponent> GetThirdPartyComponents() => _componentStorage.GetItems();
    public List<Project> GetProjects() => _projectStorage.GetItems();
    public List<ComponentCve> GetComponentCves() => _cveStorage.GetItems();
    public List<Finding> GetFindings() => _findingsStorage.GetItems();

    public void SaveTeams(List<Team> teams) => _teamStorage.SaveItems(teams);
    public void SaveWorkspace(List<Workspace> workspaces) => _workspaceStorage.SaveItems(workspaces);
    public void InsertOrUpdateWorkspace(Workspace workspace) => _workspaceStorage.InsertOrUpdate(workspace, w => w.Id == workspace.Id);
    public void InsertOrUpdateFinding(Finding finding) => _findingsStorage.InsertOrUpdate(finding, f => f.Id == finding.Id);
    public void InsertFinding(Finding finding) => _findingsStorage.Insert(finding, f => f.Id == finding.Id);
    public bool RemoveWorkspace(Guid workspaceId) => _workspaceStorage.Remove(w => w.Id == workspaceId);
    public void SaveRepositories(List<Repository> repositories) => _repositoryStorage.SaveItems(repositories);
    public string InsertOrUpdateRepository(Repository repository)
    {
        _repositoryStorage.InsertOrUpdate(repository, r => r.RepositoryId == repository.RepositoryId);
        return repository.MainBranch?.CommitId ?? "";
    }
    public bool RemoveRepository(Guid repositoryId) => _repositoryStorage.Remove(r => r.RepositoryId == repositoryId);
    public bool RemoveFinding(string findingId) => _findingsStorage.Remove(f => f.Id == findingId);
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