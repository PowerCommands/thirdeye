using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Data;

public class FindingObjects : IDataObjects<Finding>
{
    public DateTime LastUpdated { get; set; }
    public List<Finding> Items { get; set; } = [];
}