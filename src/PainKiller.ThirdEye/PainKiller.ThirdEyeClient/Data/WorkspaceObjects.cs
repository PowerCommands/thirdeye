using PainKiller.ThirdEyeClient.Contracts;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Data;

public class WorkspaceObjects : IDataObjects<Workspace>
{
    public DateTime LastUpdated { get; set; }
    public List<Workspace> Items { get; set; } = [];
}