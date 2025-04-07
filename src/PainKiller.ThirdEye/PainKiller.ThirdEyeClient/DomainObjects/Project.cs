namespace PainKiller.ThirdEyeClient.DomainObjects;

public class Project
{
    public Guid WorkspaceId { get; set; }
    public Guid RepositoryId { get; set; }
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string Sdk { get; set; } = "";
    public string Version { get; set; } = "";
    public string Framework { get; set; } = "";
    public string Language { get; set; } = "";
    public List<ThirdPartyComponent> Components { get; set; } = [];

}