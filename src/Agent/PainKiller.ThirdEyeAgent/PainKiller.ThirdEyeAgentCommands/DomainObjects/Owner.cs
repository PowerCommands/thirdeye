using System.Text.Json.Serialization;
namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;
public class Owner
{
    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }
}