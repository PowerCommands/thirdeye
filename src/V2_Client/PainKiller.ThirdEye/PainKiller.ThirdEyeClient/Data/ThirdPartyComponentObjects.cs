using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Data;

public class ThirdPartyComponentObjects : IDataObjects<ThirdPartyComponent>
{
    public DateTime LastUpdated { get; set; }
    public List<ThirdPartyComponent> Items { get; set; } = [];
}