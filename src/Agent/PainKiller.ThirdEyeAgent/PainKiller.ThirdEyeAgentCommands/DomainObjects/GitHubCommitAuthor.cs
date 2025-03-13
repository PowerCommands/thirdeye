using System.Text.Json.Serialization;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class GitHubCommitAuthor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}