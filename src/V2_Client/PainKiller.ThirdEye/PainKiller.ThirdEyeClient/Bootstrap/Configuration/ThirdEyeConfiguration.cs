using PainKiller.CommandPrompt.CoreLib.Core.Extensions;

namespace PainKiller.ThirdEyeClient.Bootstrap.Configuration;

public class ThirdEyeConfiguration
{
    private string _backupPath = "";
    public string Host { get; set; } = "";
    public string OrganizationName { get; set; } = "";
    public string[] Workspaces { get; set; } = ["*"];
    public string[] Teams { get; set; } = ["*"];
    public NvdConfiguration Nvd { get; set; } = new();
    public IgnoreConfiguration Ignores { get; set; } = new();

    public string BackupPath
    {
        get => _backupPath.GetReplacedPlaceHolderPath();
        set => _backupPath = value;
    }

    public string AccessToken { get; set; } = "TE_RepositoryToken";
    public string AccessTokenGithub { get; set; } = "TE_github_RepositoryToken";
}