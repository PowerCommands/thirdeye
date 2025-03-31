using System.Text.Json.Serialization;

namespace PainKiller.ThirdEyeClient.DomainObjects.Github;

public class GitHubTreeResponse
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = "";  // Commit SHA för denna tree
    [JsonPropertyName("tree")]
    public List<GitHubTreeItem> Tree { get; set; } = new();
}