using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;
public class AdsManager(string serverUrl, string accessToken, string[] ignoredRepositories, string[] ignoredProjects ,IConsoleWriter writer) : IGitManager
{
    private readonly VssConnection _connection = new(new Uri(serverUrl), new VssBasicCredential(string.Empty, accessToken));
    public void Connect()
    {
        var client = _connection.GetClient<ProjectHttpClient>();
        var projects = client.GetProjects().Result;
        writer.WriteSuccess($"Number of projects in organization: {projects.Count}");
    }
    public IEnumerable<Workspace> GetWorkspaces()
    {
        var client = _connection.GetClient<ProjectHttpClient>();
        return client.GetProjects().Result.Select(p => new Workspace { Description = p.Description, LastUpdateTime = p.LastUpdateTime, Name = p.Name, Revision = p.Revision, State = p.State.ToString(), Url = p.Url, Id = p.Id });
    }
    public IEnumerable<Team> GetAllTeams()
    {
        var teamClient = _connection.GetClient<TeamHttpClient>();
        var retVal = new List<Team>();
        var projects = GetWorkspaces();
        var members = new List<Member>();

        foreach (var project in projects)
        {
            var teams = GetTeams(project.Id);
            foreach (var webApiTeam in teams)
            {
                if (retVal.Any(t => t.Id == webApiTeam.Id))
                {
                    var existingTeam = retVal.First(t => t.Id == webApiTeam.Id);
                    existingTeam.WorkspaceIds.Add(project.Id);
                    retVal.Remove(existingTeam);
                    retVal.Add(existingTeam);
                    continue;
                }
                var teamMembers = teamClient.GetTeamMembersWithExtendedPropertiesAsync(project.Id.ToString(), webApiTeam.Id.ToString()).Result;
                foreach (var teamMember in teamMembers)
                {
                    if (members.Any(m => m.Id == teamMember.Identity.Id))
                    {
                        var existingMember = members.First(m => m.Id == teamMember.Identity.Id);
                        if (!existingMember.TeamIds.Contains(project.Id))
                        {
                            existingMember.TeamIds.Add(project.Id);
                            members.Remove(existingMember);
                            members.Add(existingMember);
                        }
                        continue;
                    }
                    var member = new Member { Name = teamMember.Identity.DisplayName, Url = teamMember.Identity.Url, Id = teamMember.Identity.Id, IsTeamAdmin = teamMember.IsTeamAdmin };
                    member.TeamIds.Add(webApiTeam.Id);
                    members.Add(member);
                }
                var team = new Team { Description = webApiTeam.Description, Id = webApiTeam.Id, Name = webApiTeam.Name, Url = webApiTeam.Url, Members = teamMembers.Select(m => new Member { Name = m.Identity.DisplayName, Url = m.Identity.Url, Id = m.Identity.Id, IsTeamAdmin = m.IsTeamAdmin }).ToList() };
                team.WorkspaceIds.Add(project.Id);
                retVal.Add(team);
            }
        }
        return retVal;
    }
    private IEnumerable<Team> GetTeams(Guid projectId)
    {
        var teamClient = _connection.GetClient<TeamHttpClient>();
        var retVal = new List<Team>();
        var teams = teamClient.GetTeamsAsync(projectId.ToString()).Result;
        foreach (var webApiTeam in teams)
        {
            if (retVal.Any(t => t.Id == webApiTeam.Id)) continue;
            var members = teamClient.GetTeamMembersWithExtendedPropertiesAsync(webApiTeam.ProjectId.ToString(), webApiTeam.Id.ToString()).Result;
            var team = new Team { Description = webApiTeam.Description, Id = webApiTeam.Id, Name = webApiTeam.Name, Url = webApiTeam.Url, Members = members.Select(m => new Member { Name = m.Identity.DisplayName, Url = m.Identity.Url, Id = m.Identity.Id, IsTeamAdmin = m.IsTeamAdmin }).ToList() };
            retVal.Add(team);
        }
        return retVal;
    }
    public IEnumerable<Repository> GetRepositories(Guid projectId)
    {
        try
        {
            var gitClient = _connection.GetClient<GitHttpClient>();
            var gitRepos = gitClient.GetRepositoriesAsync(projectId.ToString()).Result;
            var repos = new List<Repository>();
            foreach (var gitRepository in gitRepos)
            {
                if(ignoredRepositories.FirstOrDefault() != "*" && ignoredRepositories.Any(r => string.Equals(r, gitRepository.Name, StringComparison.CurrentCultureIgnoreCase))) continue;
                var mainBranch = GetMainBranch(gitRepository.Id);
                var repo = new Repository { Name = gitRepository.Name, WorkspaceId = projectId, RepositoryId = gitRepository.Id, Url = gitRepository.Url, MainBranch = mainBranch };
                repos.Add(repo);
            }

            return repos;
        }
        catch(Exception ex)
        {
            writer.WriteFailure($"Error fetching repository with project id: {projectId} {ex.Message}");
            return new List<Repository>();
        }
    }
    private Branch? GetMainBranch(Guid repositoryId)
    {
        try
        {
            var gitClient = _connection.GetClient<GitHttpClient>();
            var branches = gitClient.GetBranchesAsync(repositoryId.ToString()).Result;

            var baseBranch = branches.FirstOrDefault(b => b.IsBaseVersion);
            var mainBranch = (baseBranch ?? branches.FirstOrDefault(b =>
                b.Name.Equals("refs/heads/main", StringComparison.OrdinalIgnoreCase) ||
                b.Name.Equals("refs/heads/master", StringComparison.OrdinalIgnoreCase))) ?? branches.FirstOrDefault();

            return mainBranch == null ? null : new Branch { CommitId = mainBranch.Commit.CommitId, Name = mainBranch.Name, Author = mainBranch.Commit.Author.Email, IsBaseVersion = true };
        }
        catch (Exception ex)
        {
            IPowerCommandServices.DefaultInstance?.Logger.Log(LogLevel.Error, $"{ex.Message}");
            return null;
        }

    } 
    public IEnumerable<Item> GetAllFilesInRepository(Guid repositoryId)
    {
        try
        {
            var gitClient = _connection.GetClient<GitHttpClient>();
            var items = gitClient.GetItemsAsync(repositoryId.ToString(), scopePath: "/", recursionLevel: VersionControlRecursionType.Full).Result.Select(i => new Item { CommitId = i.CommitId, Content = i.Content, IsFolder = i.IsFolder, Path = i.Path, RepositoryId = repositoryId}).ToList();
            foreach (var item in items.Where(i => FileAnalyzeManager.IsRelevantFile(i.Path))) item.Content = GetContent(item, repositoryId);
            return items;
        }
        catch (Exception ex)
        {
            IPowerCommandServices.DefaultInstance?.Logger.Log(LogLevel.Error, $"{ex.Message}");
            return new List<Item>();
        }
    }
    private string GetContent(Item item, Guid repositoryId)
    {
        var gitClient = _connection.GetClient<GitHttpClient>();
        var stream = gitClient.GetItemTextAsync(repositoryId, item.Path).Result;
        if (stream == null) return string.Empty;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}