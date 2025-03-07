using Microsoft.TeamFoundation.Core.WebApi;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    /// <summary>The project's description (if any).</summary>
    public string Description { get; set; } = "";

    /// <summary>Url to the full version of the object.</summary>
    public string Url { get; set; } = "";
    public string State { get; set; } = "";
    public long Revision { get; set; }
    public ProjectVisibility Visibility { get; set; }
    public DateTime LastUpdateTime { get; set; }
}