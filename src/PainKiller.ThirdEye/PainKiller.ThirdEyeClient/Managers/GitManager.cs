using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;
using PainKiller.ThirdEyeClient.DomainObjects.Github;
using PainKiller.ThirdEyeClient.Extensions;

namespace PainKiller.ThirdEyeClient.Managers;

public class GitHubManager : IGitManager
{
    private readonly ILogger<GitHubManager> _logger = LoggerProvider.CreateLogger<GitHubManager>();

    private readonly HttpClient _client;
    private readonly Dictionary<Guid, string> _repoCache = new();
    private readonly string _serverUrl;
    private readonly string _accessToken;
    private readonly string _organizationName;
    private readonly string[] _ignoredRepositories;
    private readonly string[] _ignoredProjects;
    private readonly IConsoleWriter _writer;
    public GitHubManager(string serverUrl, string accessToken, string organizationName, string[] ignoredRepositories, string[] ignoredProjects, IConsoleWriter writer)
    {
        _serverUrl = serverUrl;
        _accessToken = accessToken;
        _organizationName = organizationName;
        _ignoredRepositories = ignoredRepositories;
        _ignoredProjects = ignoredProjects;
        _writer = writer;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Authorization", $"token {accessToken}");
        _client.DefaultRequestHeaders.Add("User-Agent", "GitHubApiRequest");
    }
    public void Connect()
    {
        if (!_client.DefaultRequestHeaders.Contains("Authorization"))
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"token {_accessToken}");
        }
        _client.DefaultRequestHeaders.Add("User-Agent", "GitHubApiRequest");
        var response = _client.GetAsync($"{_serverUrl}").Result;
        if (response.IsSuccessStatusCode)
        {
            var result = response.Content.ReadAsStringAsync().Result;
            _writer.WriteSuccessLine($"Connected to GitHub");
        }
        else
        {
            _writer.WriteError($"Failed to connect to GitHub: {response.ReasonPhrase}", nameof(Connect));
        }
    }
    public IEnumerable<Workspace> GetWorkspaces() => new List<Workspace> { new Workspace { Name = _organizationName, Description = "Github account", LastUpdateTime = DateTime.Now, Revision = 1, State = "Active", Url = _serverUrl, Id = _organizationName.GenerateGuidFromString()} };
    public IEnumerable<Team> GetAllTeams()
    {
        try
        {
            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                throw new InvalidOperationException("Authorization header is missing.");
            }

            var response = _client.GetAsync($"https://api.github.com/orgs/{_organizationName}/teams").Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API request failed with status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }

            var teams = JsonSerializer.Deserialize<List<GitHubTeam>>(response.Content.ReadAsStringAsync().Result);
            if (teams == null || teams.Count == 0) return [new Team { Name = "Default", Description = "Default team for all repositories", Url = _serverUrl, Id = Guid.NewGuid(), WorkspaceIds = [_organizationName.GenerateGuidFromString()] }];
            return teams.Select(team => new Team { Name = team.Name, Id = team.Id.ToGuid(), Url = team.HtmlUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");
        }
        return [new Team { Name = "Default", Description = "Default team for all repositories", Url = _serverUrl, Id = Guid.NewGuid(), WorkspaceIds = [_organizationName.GenerateGuidFromString()] }];
    }
    public IEnumerable<Repository> GetRepositories(Guid projectId)
    {
        var response = _client.GetStringAsync("https://api.github.com/user/repos").Result;
        var repositories = JsonSerializer.Deserialize<List<GitHubRepo>>(response) ?? [];
        var retVal = new List<Repository>();
        foreach (var gitHubRepo in repositories)
        {
            if(_ignoredRepositories.FirstOrDefault() != "*" && _ignoredRepositories.Any(r => string.Equals(r, gitHubRepo.Name, StringComparison.CurrentCultureIgnoreCase))) continue;
            retVal.Add(new Repository { Name = gitHubRepo.Name, RepositoryId = gitHubRepo.Id.ToGuid(), Url = gitHubRepo.HtmlUrl, WorkspaceId = projectId, MainBranch = new Branch{CommitId = GetCommitIdFromTree(gitHubRepo.Name) }});
        }
        return retVal;
    }
    private string GetCommitIdFromTree(string repoName)
    {
        try
        {
            var response = _client.GetStringAsync($"https://api.github.com/repos/{_organizationName}/{repoName}/git/trees/main").Result;
            var tree = JsonSerializer.Deserialize<GitHubTreeResponse>(response);
            return tree?.Sha ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");
        }
        return "";
    }
    private string GetRepositoryNameFromId(Guid repositoryId)
    {
        if (_repoCache.ContainsKey(repositoryId))
            return _repoCache[repositoryId];

        var repositories = GetRepositories(Guid.Empty); // Hämta alla repos
        foreach (var repo in repositories)
        {
            _repoCache[repo.RepositoryId] = repo.Name;
        }

        return _repoCache.TryGetValue(repositoryId, out var repoName) ? repoName : "";
    }
    public IEnumerable<Item> GetAllFilesInRepository(Guid repositoryId)
    {
        try
        {
            var repoName = GetRepositoryNameFromId(repositoryId);
            if (string.IsNullOrEmpty(repoName)) return new List<Item>();

            var response = _client.GetStringAsync($"https://api.github.com/repos/{_organizationName}/{repoName}/git/trees/main?recursive=1").Result;
            var tree = JsonSerializer.Deserialize<GitHubTreeResponse>(response);
            var items =tree?.Tree.Select(file => new Item { Path = file.Path, CommitId = tree.Sha, IsFolder = file.Type == "tree", }).ToList() ?? new List<Item>();
            foreach (var item in items.Where(i => FileAnalyzeManager.IsRelevantFile(i.Path))) item.Content = GetContent(item, repositoryId);
            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");
        }
        return new List<Item>();
    }
    private string GetContent(Item item, Guid repositoryId)
    {
        var repoName = GetRepositoryNameFromId(repositoryId);
        if (string.IsNullOrEmpty(repoName)) return "";

        var response = _client.GetStringAsync($"https://api.github.com/repos/{_organizationName}/{repoName}/contents/{item.Path}").Result;
        var fileData = JsonSerializer.Deserialize<GitHubFileResponse>(response);

        return fileData?.Content != null ? Encoding.UTF8.GetString(Convert.FromBase64String(fileData.Content)).Trim().Replace("\uFEFF", "") : "";
    }
}