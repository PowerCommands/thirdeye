using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class GitHubManager : IGitManager
{
    private readonly HttpClient _client;
    private readonly Dictionary<Guid, string> _repoCache = new();
    private readonly string _serverUrl;
    private readonly string _accessToken;
    private readonly string _organizationName;
    private readonly IConsoleWriter _writer;
    public GitHubManager(string serverUrl, string accessToken, string organizationName, IConsoleWriter writer)
    {
        _serverUrl = serverUrl;
        _accessToken = accessToken;
        _organizationName = organizationName;
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
            _writer.WriteSuccess($"Connected to GitHub");
        }
        else
        {
            _writer.WriteFailure($"Failed to connect to GitHub: {response.ReasonPhrase}");
        }
    }
    public IEnumerable<Workspace> GetWorkspaces() => new List<Workspace> { new Workspace { Name = _organizationName, Description = "Github account", LastUpdateTime = DateTime.Now, Revision = 1, State = "Active", Url = _serverUrl, Id = Guid.NewGuid()} };
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
            return teams?.Select(team => new Team { Name = team.Name, Id = team.Id.ToGuid(), Url = team.HtmlUrl }) ?? new List<Team>();
        }
        catch (Exception ex)
        {
            PowerCommandServices.Service.Logger.Log(LogLevel.Error, $"Error in GetAllTeams: {ex.Message}");
        }

        return new List<Team>();
    }
    public IEnumerable<Repository> GetRepositories(Guid projectId)
    {
        var response = _client.GetStringAsync("https://api.github.com/user/repos").Result;
        var repositories = JsonSerializer.Deserialize<List<GitHubRepo>>(response);

        return  repositories?.Select(repo => new Repository { Name = repo.Name, RepositoryId = repo.Id.ToGuid(), Url = repo.HtmlUrl, WorkspaceId = projectId}) ?? new List<Repository>();
        
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
            var items =tree?.Tree?.Select(file => new Item { Path = file.Path, CommitId = tree.Sha, IsFolder = file.Type == "tree", }).ToList() ?? new List<Item>();
            foreach (var item in items.Where(i => FileAnalyzeManager.IsRelevantFile(i.Path))) item.Content = GetContent(item, repositoryId);
            return items;
        }
        catch (Exception ex)
        {
            PowerCommandServices.Service.Logger.Log(LogLevel.Error, $"Error in GetAllFilesInRepository: {ex.Message}");
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