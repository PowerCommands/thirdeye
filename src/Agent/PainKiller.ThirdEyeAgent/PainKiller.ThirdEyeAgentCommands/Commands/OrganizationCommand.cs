﻿using PainKiller.ThirdEyeAgentCommands.Extensions;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Shows an overview of your organization, based on what you have configured to fetch.",
                  disableProxyOutput: true,
                             example: "//Show hierarchy of your organization|organization")]
    public class OrganizationCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var projects = Storage.GetWorkspaces().GetFilteredProjects(Configuration.ThirdEyeAgent.Workspaces);
            var repositories = Storage.GetRepositories();
            var teams = Storage.GetTeams();
            var devProjects = Storage.GetDevProjects();
            PresentationManager.DisplayOrganization(Configuration.ThirdEyeAgent.OrganizationName, projects, repositories, teams, devProjects);
            EnableLog();
            return Ok();
        }
    }
}