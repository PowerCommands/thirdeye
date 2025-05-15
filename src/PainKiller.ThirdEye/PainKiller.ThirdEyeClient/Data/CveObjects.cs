namespace PainKiller.ThirdEyeClient.Data;

public class CveObjects
{
    public DateTime LastUpdated { get; set; }
    public List<CveEntry> Entries { get; set; } = [];
}