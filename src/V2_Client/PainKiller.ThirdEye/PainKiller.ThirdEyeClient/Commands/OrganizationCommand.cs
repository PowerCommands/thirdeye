using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Extensions;

namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign(description: "Shows an overview of your organization, based on what you have configured to fetch.",
    arguments: [],
    examples: ["//Show hierarchy of your organization", "organization"])]
public class OrganizationCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var workspaces = Storage.GetWorkspaces().GetFilteredWorkspaces(Configuration.ThirdEye.Workspaces);
        var repositories = Storage.GetRepositories();
        var teams = Storage.GetTeams();
        var projects = Storage.GetProjects();
        PresentationManager.DisplayOrganization(Configuration.ThirdEye.OrganizationName, workspaces, repositories, teams, projects, null);
        return Ok();
    }
}
