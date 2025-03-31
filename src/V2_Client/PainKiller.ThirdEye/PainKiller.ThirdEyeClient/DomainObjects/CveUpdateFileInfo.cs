namespace PainKiller.ThirdEyeClient.DomainObjects;

public class CveUpdateFileInfo
{
    public DateTime Created { get; set; }
    public int CveCount { get; set; }
    public string Checksum { get; set; } = "";
}