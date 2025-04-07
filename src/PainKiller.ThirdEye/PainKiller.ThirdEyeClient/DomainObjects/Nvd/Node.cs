namespace PainKiller.ThirdEyeClient.DomainObjects.Nvd;

public class Node
{
    public string _operator { get; set; }
    public bool negate { get; set; }
    public Cpematch[] cpeMatch { get; set; }
}