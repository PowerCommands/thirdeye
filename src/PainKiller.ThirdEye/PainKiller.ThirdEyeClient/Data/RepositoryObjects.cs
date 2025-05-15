using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Data;

public class RepositoryObjects : IDataObjects<Repository>
{
    public DateTime LastUpdated { get; set; }
    public List<Repository> Items { get; set; } = [];
}