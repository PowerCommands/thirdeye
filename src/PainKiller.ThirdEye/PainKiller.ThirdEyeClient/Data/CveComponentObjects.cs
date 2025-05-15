using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Data;
public class CveComponentObjects : IDataObjects<ComponentCve>
{
    public DateTime LastUpdated { get; set; }
    public List<ComponentCve> Items { get; set; } = [];
    public CvssSeverity Severity { get; set; }
}