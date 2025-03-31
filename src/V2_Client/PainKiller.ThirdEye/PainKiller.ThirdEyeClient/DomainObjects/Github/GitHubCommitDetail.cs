using System.Text.Json.Serialization;

namespace PainKiller.ThirdEyeClient.DomainObjects.Github;

public class GitHubCommitDetail
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public GitHubCommitAuthor Author { get; set; } = new();
}