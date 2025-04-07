namespace PainKiller.ThirdEyeClient.DomainObjects;
public class Repository
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public Guid WorkspaceId { get; set; }
    public Guid RepositoryId { get; set; }
    public Branch? MainBranch { get; set; } = new();
    public bool IsGit { get; set; }
}