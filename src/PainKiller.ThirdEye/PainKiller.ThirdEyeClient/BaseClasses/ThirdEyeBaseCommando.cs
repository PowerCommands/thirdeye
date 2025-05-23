using System.ComponentModel;
using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;
using PainKiller.CommandPrompt.CoreLib.Modules.ShellModule.Services;
using PainKiller.ThirdEyeClient.Configuration;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;
using PainKiller.ThirdEyeClient.Managers;

namespace PainKiller.ThirdEyeClient.BaseClasses;
public abstract class ThirdEyeBaseCommando(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override void OnInitialized()
    {
        var config = Configuration.ThirdEye;
        var gitHostType = config.Host.GetGitHostType();
        var gitHub = config.Host.Contains("github.com");
        var accessToken = Configuration.Core.Modules.Security.DecryptSecret(gitHub ? Configuration.ThirdEye.AccessTokenGithub : Configuration.ThirdEye.AccessToken);
        var ignoreRepositories = config.Ignores.Repositories;
        var ignoreProjects = config.Ignores.Projects;
        
        Storage = ObjectStorageService.Service;
        
        if(gitHostType == GitHostType.Ads) GitManager = new AdsManager(config.Host, accessToken, ignoreRepositories, ignoreProjects,Writer);
        else if (gitHostType == GitHostType.Github) GitManager = new GitHubManager(config.Host, accessToken, config.OrganizationName, ignoreRepositories, ignoreProjects, Writer);
        else GitManager = new LocalDirectoryGitManager(config.Host, Environment.MachineName, Writer);
        PresentationManager = new PresentationManager(Writer);
        CveStorage = CveStorageService.Service;
    }
    protected ICveStorageService CveStorage { get; private set; } = null!;
    protected IObjectStorageService Storage { get; private set; } = null!;
    protected IGitManager GitManager { get; private set; } = null!;
    protected IFileAnalyzeManager AnalyzeManager { get; } = new FileAnalyzeManager();
    protected PresentationManager PresentationManager { get; private set; } = null!;
    protected void ProjectSearch(ThirdPartyComponent component, bool detailedSearch)
    {
        var projects = Storage.GetProjects().Where(dp => dp.Components.Any(c => c.Name == component.Name && (c.Version == component.Version || !detailedSearch))).ToList();
        var workspaces = Storage.GetWorkspaces().Where(p => projects.Any(dp => dp.WorkspaceId == p.Id)).ToList();
        var repos = new List<Repository>();
        var teams = Storage.GetTeams();
        foreach (var projectRepos in workspaces.Select(project => Storage.GetRepositories().Where(r => r.WorkspaceId == project.Id)))
        {
            foreach (var projectRepo in projectRepos)
            {
                if(projects.Any(p => p.RepositoryId == projectRepo.RepositoryId))
                    repos.Add(projectRepo);
            }
        }
        PresentationManager.DisplayOrganization(Configuration.ThirdEye.OrganizationName, workspaces, repos, teams, projects, component, skipEmpty: true);
        var fileName = CreateReport(repos, projects, component.Name);
        Writer.WriteSuccessLine($"Successfully write a report to file: {fileName}");
    }

    protected void ComponentSearch(ThirdPartyComponent component, bool detailedSearch)
    {
        var projects = Storage.GetProjects().Where(dp => dp.Components.Any(c => c.Name == component.Name && (c.Version == component.Version || !detailedSearch))).ToList();
        var workspaces = Storage.GetWorkspaces().Where(p => projects.Any(dp => dp.WorkspaceId == p.Id)).ToList();
        var repos = new List<Repository>();
        var teams = Storage.GetTeams();
        foreach (var projectRepos in workspaces.Select(project => Storage.GetRepositories().Where(r => r.WorkspaceId == project.Id)))
        {
            foreach (var projectRepo in projectRepos)
            {
                if(projects.Any(p => p.RepositoryId == projectRepo.RepositoryId))
                    repos.Add(projectRepo);
            }
        }
        PresentationManager.DisplayComponents(Configuration.ThirdEye.OrganizationName, workspaces, repos, teams, projects, component, skipEmpty: true);
        var fileName = CreateReport(repos, projects, component.Name);
        Writer.WriteSuccessLine($"Successfully write a report to file: {fileName}");
    }
    protected string CreateReport(List<Repository> repos, List<Project> projects, string filter)
    {
        var reportManager = new ReportManager(Path.Combine(AppContext.BaseDirectory, $"component_report"));
        var fileName = reportManager.Create(Configuration.ThirdEye.OrganizationName, repos, projects, filter);
        ShellService.Default.OpenWithDefaultProgram(fileName);
        return fileName;
    }
}