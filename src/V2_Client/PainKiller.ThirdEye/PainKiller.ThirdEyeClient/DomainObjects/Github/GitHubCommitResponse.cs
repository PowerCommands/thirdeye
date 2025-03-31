using System.Text.Json.Serialization;

namespace PainKiller.ThirdEyeClient.DomainObjects.Github;

public class GitHubCommitResponse
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("commit")]
    public GitHubCommitDetail Commit { get; set; } = new();
}