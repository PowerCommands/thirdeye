using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;
using PainKiller.ThirdEyeClient.Configuration;

namespace PainKiller.ThirdEyeClient.Commands;
[CommandDesign(description: "Create an access token",
    arguments: ["!<token>"],
    options: ["github"],
    examples: ["//Create a token", "token"])]
public class TokenCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var token = $"{input.Arguments.FirstOrDefault()}".ToLower();
        var accessTokenName = input.HasOption("githug") ? Configuration.ThirdEye.AccessTokenGithub : Configuration.ThirdEye.AccessToken;
        Configuration.Core.Modules.Security.EncryptSecret(EnvironmentVariableTarget.User, accessTokenName, token);
        Writer.WriteSuccessLine("\nToken has been created");
        return Ok();
    }
}
