namespace PainKiller.ThirdEyeAgentCommands.Configuration;

public class ThirdEyeConfiguration
{
    public string Host { get; set; } = "";
    public string OrganizationName { get; set; } = "";
    public string[] Workspaces { get; set; } = ["*"];
    public string[] Teams { get; set; } = ["*"];
    public NvdConfiguration Nvd { get; set; } = new();
    public IgnoreConfiguration Ignores { get; set; } = new();
}