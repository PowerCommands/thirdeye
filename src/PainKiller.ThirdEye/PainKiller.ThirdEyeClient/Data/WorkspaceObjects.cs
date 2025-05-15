using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Data;

public class WorkspaceObjects : IDataObjects<Workspace>
{
    public DateTime LastUpdated { get; set; }
    public List<Workspace> Items { get; set; } = [];
}