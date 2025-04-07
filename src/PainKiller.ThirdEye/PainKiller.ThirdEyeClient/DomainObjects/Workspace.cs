namespace PainKiller.ThirdEyeClient.DomainObjects;

public class Workspace
{
    public Workspace() { }
    public Workspace(string description, DateTime lastUpdateTime, string name, long revision, string state, string url, Guid id)
    {
        Description = description;
        LastUpdateTime = lastUpdateTime;
        Name = name;
        Revision = revision;
        State = state;
        Url = url;
        Id = id;
    }
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    /// <summary>The project's description (if any).</summary>
    public string Description { get; set; } = "";

    /// <summary>Url to the full version of the object.</summary>
    public string Url { get; set; } = "";
    public string State { get; set; } = "";
    public long Revision { get; set; }
    public DateTime LastUpdateTime { get; set; }
}