namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;


public class CveDetailResponse
{
    public string CveId { get; set; } = "";
    public int resultsPerPage { get; set; }
    public int startIndex { get; set; }
    public int totalResults { get; set; }
    public string format { get; set; }
    public string version { get; set; }
    public DateTime timestamp { get; set; }
    public Vulnerability[] vulnerabilities { get; set; }
}