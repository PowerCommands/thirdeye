namespace PainKiller.ThirdEyeAgentCommands.Configuration;

public class NvdConfiguration
{
    public string Url { get; set; } = "https://services.nvd.nist.gov/rest/json/cves/2.0";
    public int PageSize { get; set; } = 2000;
    public int DelayIntervalSeconds { get; set; } = 10;
}