using System.Text.Json.Serialization;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;
public class GitHubRepo
{
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = "";
    [JsonPropertyName("language")]
    public string Language { get; set; } = "";

    [JsonPropertyName("owner")]
    public Owner Owner { get; set; } = new();
}
