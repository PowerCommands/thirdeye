namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Setup your configuration",
                  disableProxyOutput: true,
                             example: "//Setup your configuration|setup")]
    public class SetupCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var teams = GitManager.GetAllTeams().ToList();
            var (key, _) = ListService.ListDialog("Choose your team", teams.Select(p => $"{p.Name,-50} {p.Id}").ToList(), autoSelectIfOnlyOneItem: false).First();
            var selectedTeam = teams[key];
            var allProjects = GitManager.GetProjects().ToList();
            var teamProjects = allProjects.Where(p => selectedTeam.ProjectIds.Any(t => t == p.Id)).ToList();
            var selectedProjects = ListService.ListDialog("Choose your projects", teamProjects.Select(p => p.Name).ToList(), multiSelect: true, autoSelectIfOnlyOneItem: false);
            configuration.ThirdEyeAgent.Teams = [selectedTeam.Name];
            configuration.ThirdEyeAgent.Projects = selectedProjects.Count == 0 ? ["*"] : selectedProjects.Select(p => p.Value).ToArray();

            var host = DialogService.QuestionAnswerDialog("Enter you host:", "Host (url to server):");
            var organizationName = DialogService.QuestionAnswerDialog("Enter your organization (or github user) name:","Organization name:");
            configuration.ThirdEyeAgent.Host = host;
            configuration.ThirdEyeAgent.OrganizationName = organizationName;
            ConfigurationService.Service.SaveChanges(configuration);
            return Ok();
        }
    }
}