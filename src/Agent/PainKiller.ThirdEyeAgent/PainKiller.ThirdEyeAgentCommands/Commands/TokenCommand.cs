namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Create a access token",
                           arguments: "!<token>",
                             options: "github",
                  disableProxyOutput: true,
                             example: "//Create a token|token")]
    [PowerCommandPrivacy]
    public class TokenCommand(string identifier, PowerCommandsConfiguration configuration) : CommandBase<PowerCommandsConfiguration>(identifier, configuration)
    {
        public override RunResult Run()
        {
            var token = Input.SingleArgument;
            Configuration.EncryptSecret(EnvironmentVariableTarget.User, ConfigurationGlobals.GetAccessTokenName(HasOption("github")), token);
            WriteSuccessLine($"\nToken has been created");
            return Ok();
        }
    }
}