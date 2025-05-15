using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Data;

public class SoftwareObjects : IDataObjects<Software>
{
    public DateTime LastUpdated { get; set; }
    public List<Software> Items { get; set; } = [];
}