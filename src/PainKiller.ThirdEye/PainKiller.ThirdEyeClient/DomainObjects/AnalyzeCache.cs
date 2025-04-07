using PainKiller.ThirdEyeClient.Enums;

namespace PainKiller.ThirdEyeClient.DomainObjects;

public class AnalyzeCache
{
    public List<ThirdPartyComponent> Components { get; set; }
    public CvssSeverity Severity { get; set; }
}