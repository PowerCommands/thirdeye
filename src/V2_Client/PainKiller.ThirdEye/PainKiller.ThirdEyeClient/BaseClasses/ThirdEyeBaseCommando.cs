﻿using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;
using PainKiller.ThirdEyeClient.Bootstrap;
using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;
using PainKiller.ThirdEyeClient.Enums;
using PainKiller.ThirdEyeClient.Extensions;
using PainKiller.ThirdEyeClient.Managers;
using PainKiller.ThirdEyeClient.Services;

namespace PainKiller.ThirdEyeClient.BaseClasses;
public abstract class ThirdEyeBaseCommando : ConsoleCommandBase<CommandPromptConfiguration>
{
    private readonly List<Repository> _analyzedRepositories = [];
    protected ThirdEyeBaseCommando(string identifier) : base(identifier)
    {
        var config = Configuration.ThirdEye;
        var gitHostType = config.Host.GetGitHostType();
        var gitHub = config.Host.Contains("github.com");
        var accessToken = Configuration.Core.Modules.Security.DecryptSecret(gitHub ? Configuration.ThirdEye.AccessTokenGithub : Configuration.ThirdEye.AccessToken);
        var ignoreRepositories = config.Ignores.Repositories;
        var ignoreProjects = config.Ignores.Projects;
        ObjectStorageService.Initialize(config.Host);
        Storage = ObjectStorageService.Service;
        
        if(gitHostType == GitHostType.Ads) GitManager = new AdsManager(config.Host, accessToken, ignoreRepositories, ignoreProjects,this);
        else if (gitHostType == GitHostType.Github) GitManager = new GitHubManager(config.Host, accessToken, config.OrganizationName, ignoreRepositories, ignoreProjects, this);
        else GitManager = new LocalDirectoryGitManager(config.Host, Environment.MachineName, this);
        
        PresentationManager = new PresentationManager(Writer);
        CveStorageService.Initialize(config.Nvd.PathToUpdates);
        CveStorage = CveStorageService.Service;
    }
    protected ICveStorageService CveStorage { get; init; }
    protected IObjectStorageService Storage { get; }
    protected IGitManager GitManager { get; }
    protected IFileAnalyzeManager AnalyzeManager { get; } = new FileAnalyzeManager();
    protected PresentationManager PresentationManager { get; }
    
    
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
    }
}