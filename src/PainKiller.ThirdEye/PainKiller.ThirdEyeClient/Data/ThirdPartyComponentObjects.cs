using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Data;

public class ThirdPartyComponentObjects : IDataObjects<ThirdPartyComponent>
{
    public DateTime LastUpdated { get; set; }
    public List<ThirdPartyComponent> Items { get; set; } = [];
}