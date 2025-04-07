using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Data;

public class SoftwareObjects : IDataObjects<Software>
{
    public DateTime LastUpdated { get; set; }
    public List<Software> Items { get; set; } = [];
}