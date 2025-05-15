using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Managers;

public class LocalDirectoryGitManager(string path, string description, IConsoleWriter writer) : IGitManager
{
    public void Connect()
    {
        if (Directory.Exists(path)) writer.WriteSuccessLine($"{path} exists!");
        else writer.WriteError($"{path} does not exists!", nameof(Connect));
    }
    public IEnumerable<Workspace> GetWorkspaces()
    {
        var retVal = new List<Workspace>();
        if (!Directory.Exists(path)) return retVal;
        
        var rootDir = new DirectoryInfo(path);
        retVal.Add(new Workspace { Description = rootDir.FullName, Id = description.GenerateGuidFromString(), Name = rootDir.Name, Url = rootDir.FullName });
        return retVal;
    }
    public IEnumerable<Team> GetAllTeams() => [new Team { Name = "Default", Description = "Default team for all repositories", Url = path, Id = Guid.NewGuid(), WorkspaceIds = [description.GenerateGuidFromString()] }];
    public IEnumerable<Repository> GetRepositories(Guid workspaceId)
    {
        var retVal = new List<Repository>();
        if (!Directory.Exists(path)) return retVal;
        var rootDir = new DirectoryInfo(path);
        retVal.AddRange(rootDir.GetDirectories().Select(directory => new Repository() { RepositoryId = directory.FullName.GenerateGuidFromString(), Name = directory.Name, Url = directory.FullName, IsGit = true, WorkspaceId = description.GenerateGuidFromString() }));
        return retVal;
    }
    public IEnumerable<Item> GetAllFilesInRepository(Guid repositoryId)
    {
        try
        {
            var repo = GetRepositories(repositoryId).First(r => r.RepositoryId == repositoryId);
            var directory = new DirectoryInfo(repo.Url);
            if (!directory.Exists) return new List<Item>();

            var allFiles = directory.GetFiles("*", SearchOption.AllDirectories);
            var items = allFiles.Select(file =>
            {
                var relativePath = Path.GetRelativePath(directory.FullName, file.FullName).Replace("\\", "/");
                return new Item
                {
                    Path = relativePath,
                    CommitId = "", // optional, can be empty or maybe hash of file content or last modified date
                    IsFolder = false,
                    Content = FileAnalyzeManager.IsRelevantFile(relativePath) ? File.ReadAllText(file.FullName) : ""
                };
            }).ToList();

            var folders = directory.GetDirectories("*", SearchOption.AllDirectories)
                .Select(dir =>
                {
                    var relativePath = Path.GetRelativePath(directory.FullName, dir.FullName).Replace("\\", "/");
                    return new Item
                    {
                        Path = relativePath,
                        CommitId = "",
                        IsFolder = true
                    };
                });

            return items.Concat(folders);
        }
        catch (Exception ex)
        {
            writer.WriteError(ex.Message, nameof(GetAllFilesInRepository));
        }
        return new List<Item>();
    }
}