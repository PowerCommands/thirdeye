using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Data;

public class RepositoryObjects : IDataObjects<Repository>
{
    public DateTime LastUpdated { get; set; }
    public List<Repository> Items { get; set; } = [];
}