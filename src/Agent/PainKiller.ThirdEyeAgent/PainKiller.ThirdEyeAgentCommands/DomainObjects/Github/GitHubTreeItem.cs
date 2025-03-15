using System.Text.Json.Serialization;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.Github;

public class GitHubTreeItem
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = "";  // Filens fullständiga sökväg
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";  // "blob" = fil, "tree" = mapp
}