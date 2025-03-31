namespace PainKiller.ThirdEyeClient.DomainObjects.Nvd;

public class Cvssmetricv31
{
    public string source { get; set; }
    public string type { get; set; }
    public Cvssdata cvssData { get; set; }
    public float exploitabilityScore { get; set; }
    public float impactScore { get; set; }
}