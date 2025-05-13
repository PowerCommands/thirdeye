namespace PainKiller.ThirdEyeClient.DomainObjects;
public class Item
{
    public Guid RepositoryId { get; set; }
    public string CommitId { get; set; } = "";
    public string Path { get; set; } = "";
    public bool IsFolder { get; set; }
    public string Content { get; set; } = "";
    public string UserId { get; set; } = "";
}