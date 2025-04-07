using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Data;

public class ProjectObjects : IDataObjects<Project>
{
    public DateTime LastUpdated { get; set; }
    public List<Project> Items { get; set; } = [];
}