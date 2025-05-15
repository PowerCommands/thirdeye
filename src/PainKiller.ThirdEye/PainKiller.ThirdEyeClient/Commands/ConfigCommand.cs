using PainKiller.ThirdEyeClient.Bootstrap;

namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign( description: "Information about configuration.",
    examples: ["//Show db information","db"])]
public class ConfigCommand(string identifier) :ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        Writer.Clear();
        Startup.ShowLogo(Configuration.Core);

        Writer.WriteHeadLine($"{Emo.Settings.Icon()} Configuration");
        Writer.WriteDescription($"├── Host: ", Configuration.ThirdEye.Host, noBorder: true);
        Writer.WriteDescription($"├── Organization: ", Configuration.ThirdEye.OrganizationName, noBorder: true);
        foreach (var team in Configuration.ThirdEye.Teams) Writer.WriteDescription($"├──{Emo.Team.Icon()} Team", team, noBorder: true);
        foreach (var workspace in Configuration.ThirdEye.Workspaces) Writer.WriteDescription($"├──{Emo.Workspace.Icon()} Workspace", workspace, noBorder: true);
        foreach (var project in Configuration.ThirdEye.Ignores.Projects) Writer.WriteDescription($"├──{Emo.Filter.Icon()} Ignore project", project, noBorder: true);
        foreach (var repository in Configuration.ThirdEye.Ignores.Repositories) Writer.WriteDescription($"├──{Emo.Filter.Icon()} Ignore repo", repository, noBorder: true);
            
        Writer.WriteHeadLine($"{Emo.Bug.Icon()} NVD");
        Writer.WriteDescription($"├── Url: ", Configuration.ThirdEye.Nvd.Url, noBorder: true);
        Writer.WriteDescription($"├── Path to updates: ", Configuration.ThirdEye.Nvd.PathToUpdates, noBorder: true);
        Writer.WriteDescription($"├── Path to backups: ", Configuration.ThirdEye.BackupPath, noBorder: true);
        Writer.WriteDescription($"├── DelayIntervalSeconds: ", $"{Configuration.ThirdEye.Nvd.DelayIntervalSeconds}", noBorder: true);
        Writer.WriteDescription($"├── PageSize: ", $"{Configuration.ThirdEye.Nvd.PageSize}", noBorder: true);
        Writer.WriteDescription($"├── TimeoutSeconds: ", $"{Configuration.ThirdEye.Nvd.TimeoutSeconds}", noBorder: true);

        var authorizationToken = Environment.GetEnvironmentVariable(Configuration.ThirdEye.AccessToken, EnvironmentVariableTarget.User);
        var gitToken = Environment.GetEnvironmentVariable(Configuration.ThirdEye.AccessTokenGithub, EnvironmentVariableTarget.User);
        var nvdApiKey = Environment.GetEnvironmentVariable(Configuration.ThirdEye.Nvd.TokenName, EnvironmentVariableTarget.User);
        if(!string.IsNullOrEmpty(authorizationToken)) Writer.WriteSuccessLine("Authorization token found:");
        if (!string.IsNullOrEmpty(gitToken)) Writer.WriteSuccessLine($"{"Git token found:", -26}");
        if (!string.IsNullOrEmpty(nvdApiKey)) Writer.WriteSuccessLine($"{"NVD API key found:", -26}");

        return Ok();
    }
}