using PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;

namespace PainKiller.ThirdEyeClient.DomainObjects.Nvd;

public class Cve
{
    public string id { get; set; }
    public string sourceIdentifier { get; set; }
    public DateTime published { get; set; }
    public DateTime lastModified { get; set; }
    public string vulnStatus { get; set; }
    public object[] cveTags { get; set; }
    public Description[] descriptions { get; set; }
    public Metrics metrics { get; set; }
    public Weakness[] weaknesses { get; set; }
    public Configuration[] configurations { get; set; }
    public Reference[] references { get; set; }
}