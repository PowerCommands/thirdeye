using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Modules.SecurityModule.Extensions;
using PainKiller.ThirdEyeClient.Configuration;

namespace PainKiller.ThirdEyeClient.Commands;

public class SetupCommand(string identifier) : ConsoleCommandBase<CommandPromptConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var authorizationToken = Environment.GetEnvironmentVariable(Configuration.ThirdEye.AccessToken, EnvironmentVariableTarget.User);
        var gitToken = Environment.GetEnvironmentVariable(Configuration.ThirdEye.AccessTokenGithub, EnvironmentVariableTarget.User);
        var nvdApiKey = Environment.GetEnvironmentVariable(Configuration.ThirdEye.Nvd.TokenName, EnvironmentVariableTarget.User);

        if (!string.IsNullOrEmpty(authorizationToken)) Writer.WriteSuccessLine("Authorization token found! ✅");
        else if (!Configuration.ThirdEye.Host.Contains("github"))
        {
            Writer.WriteHeadLine("Authorization is needed to fetch your repositories.");
            var setupAuthorizationToken = DialogService.YesNoDialog("Do you want to setup an authorization token now?");
            if (setupAuthorizationToken)
            {
                var token = DialogService.GetSecret("Please input your access token:");
                Configuration.Core.Modules.Security.EncryptSecret(EnvironmentVariableTarget.User, Configuration.ThirdEye.AccessToken, token);
                Writer.WriteSuccessLine($"Token has been created");
            }
        }
        if (!string.IsNullOrEmpty(gitToken)) Writer.WriteSuccessLine($"{"Git token found:",-26} ✅");
        else if (!Configuration.ThirdEye.Host.Contains("github"))
        {
            Writer.WriteHeadLine("Git token is needed to fetch your github repositories.");
            var setupGitToken = DialogService.YesNoDialog("Do you want to setup a git token now?");
            if (setupGitToken)
            {
                var token = DialogService.GetSecret("Please input your git token:");
                Configuration.Core.Modules.Security.EncryptSecret(EnvironmentVariableTarget.User, Configuration.ThirdEye.AccessTokenGithub, token);
                Writer.WriteSuccessLine($"\nToken has been created");
            }
        }
        if (!string.IsNullOrEmpty(nvdApiKey)) Writer.WriteSuccessLine($"{"NVD API key found:",-26} ✅");
        else
        {
            Writer.WriteHeadLine("NVD API key is you need to fetch CVEs direct from NVD API. (a local DB will be used otherwise)");
            var setupNvdApiKey = DialogService.YesNoDialog("Do you want to setup a NVD API key now");
            if (setupNvdApiKey)
            {
                var apiKey = DialogService.GetSecret("Please input your NVD API key:");
                Configuration.Core.Modules.Security.EncryptSecret(EnvironmentVariableTarget.User, Configuration.ThirdEye.Nvd.TokenName, apiKey);
                Writer.WriteSuccessLine($"\nApiKey has been created, you can now fetch CVEs from NVD.");
            }
        }
        return Ok();
    }
}