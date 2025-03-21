using PainKiller.ThirdEyeAgentCommands.BaseClasses;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Setup your configuration",
                  disableProxyOutput: true,
                             example: "//Setup your configuration|setup")]
    public class SetupCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var authorizationToken = EnvironmentService.Service.GetEnvironmentVariable(ConfigurationGlobals.GetAccessTokenName(false));
            var gitToken = EnvironmentService.Service.GetEnvironmentVariable(ConfigurationGlobals.GetAccessTokenName(true));
            var nvdApiKey = EnvironmentService.Service.GetEnvironmentVariable(ConfigurationGlobals.NvdApiKeyName);

            if(!string.IsNullOrEmpty(authorizationToken)) WriteSuccessLine("Authorization token found!");
            else if(!Configuration.ThirdEyeAgent.Host.Contains("github"))
            {
                WriteHeadLine("Authorization is needed to fetch your repositories.");
                var setupAuthorizationToken = DialogService.YesNoDialog("Do you want to setup an authorization token now?");
                if (setupAuthorizationToken)
                {
                    var token = DialogService.SecretPromptDialog("Please input your access token:");
                    Configuration.EncryptSecret(EnvironmentVariableTarget.User, ConfigurationGlobals.GetAccessTokenName(false), token);
                    WriteSuccessLine($"\nToken has been created");
                }
            }
            if (!string.IsNullOrEmpty(gitToken)) WriteSuccessLine($"{"Git token found:", -26} ✅");
            else if(!Configuration.ThirdEyeAgent.Host.Contains("github"))
            {
                WriteHeadLine("Git token is needed to fetch your github repositories.");
                var setupGitToken = DialogService.YesNoDialog("Do you want to setup a git token now?");
                if (setupGitToken)
                {
                    var token = DialogService.SecretPromptDialog("Please input your git token:");
                    Configuration.EncryptSecret(EnvironmentVariableTarget.User, ConfigurationGlobals.GetAccessTokenName(true), token);
                    WriteSuccessLine($"\nToken has been created");
                }
            }
            if (!string.IsNullOrEmpty(nvdApiKey)) WriteSuccessLine($"{"NVD API key found:", -26} ✅");
            else
            {
                WriteHeadLine("NVD API key is you need to fetch CVEs direct from NVD API. (a local DB will be used otherwise)");
                var setupNvdApiKey = DialogService.YesNoDialog("Do you want to setup a NVD API key now");
                if (setupNvdApiKey)
                {
                    var apiKey = DialogService.SecretPromptDialog("Please input your NVD API key:");
                    Configuration.EncryptSecret(EnvironmentVariableTarget.User, ConfigurationGlobals.NvdApiKeyName, apiKey);
                    WriteSuccessLine($"\nApiKey has been created, you can now fetch CVEs from NVD.");
                }
            }
            if ((!string.IsNullOrEmpty(authorizationToken) || !string.IsNullOrEmpty(gitToken)) && !string.IsNullOrEmpty(nvdApiKey))
            {
                var continueWithSetup = DialogService.YesNoDialog("Do you want to continue with setup of Team and project details?");
                if (!continueWithSetup) return Ok();
            }
            var teams = GitManager.GetAllTeams().ToList();
            var (key, _) = ListService.ListDialog("Choose your team", teams.Select(p => $"{p.Name,-50} {p.Id}").ToList(), autoSelectIfOnlyOneItem: false).First();
            var selectedTeam = teams[key];
            var allProjects = GitManager.GetWorkspaces().ToList();
            var teamProjects = allProjects.Where(p => selectedTeam.WorkspaceIds.Any(t => t == p.Id)).ToList();
            var selectedProjects = ListService.ListDialog("Choose your projects", teamProjects.Select(p => p.Name).ToList(), multiSelect: true, autoSelectIfOnlyOneItem: false);
            Configuration.ThirdEyeAgent.Teams = [selectedTeam.Name];
            Configuration.ThirdEyeAgent.Workspaces = selectedProjects.Count == 0 ? ["*"] : selectedProjects.Select(p => p.Value).ToArray();

            var host = DialogService.QuestionAnswerDialog("Enter you host:", "Host (url to server):");
            var organizationName = DialogService.QuestionAnswerDialog("Enter your organization (or github user) name:","Organization name:");
            Configuration.ThirdEyeAgent.Host = host;
            Configuration.ThirdEyeAgent.OrganizationName = organizationName;
            ConfigurationService.Service.SaveChanges(Configuration);
            return Ok();
        }
    }
}