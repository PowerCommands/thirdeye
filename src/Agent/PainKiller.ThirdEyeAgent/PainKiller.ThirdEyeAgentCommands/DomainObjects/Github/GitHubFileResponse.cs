using System.Text.Json.Serialization;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.Github;
public class GitHubFileResponse
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = ""; // Base64-kodat filinnehåll
    [JsonPropertyName("encoding")]
    public string Encoding { get; set; } = ""; // Filens kodning, vanligtvis "base64"
    [JsonPropertyName("path")]
    public string Path { get; set; } = ""; // Filens sökväg i repot
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = ""; // SHA-hash för filen
}
