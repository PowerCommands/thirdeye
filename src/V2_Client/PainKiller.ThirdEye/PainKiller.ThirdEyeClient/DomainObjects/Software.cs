using System.Text.RegularExpressions;

namespace PainKiller.ThirdEyeClient.DomainObjects;

public class Software
{
    public Software() { }
    public Software(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return;
        var match = Regex.Match(fullName, @"^(.*?)(?:\s+(\d+(\.\d+)*))?(?:\s*\((.*?)\))?$");
        Name = match.Groups[1].Value.Trim();
        Version = match.Groups[2].Success ? match.Groups[2].Value.Trim() : "";
        MetaInformation = match.Groups[4].Success ? match.Groups[4].Value.Trim() : "";
        
        if (string.IsNullOrEmpty(Name)) Name = fullName;
    }
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public string MetaInformation { get; set; } = "";
}