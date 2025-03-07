namespace PainKiller.ThirdEyeAgentCommands.Configuration;

public class ThirdEyeConfiguration
{
    public string Host { get; set; } = "";
    public string[] Projects { get; set; } = ["*"];
    public string[] Teams { get; set; } = ["*"];
}