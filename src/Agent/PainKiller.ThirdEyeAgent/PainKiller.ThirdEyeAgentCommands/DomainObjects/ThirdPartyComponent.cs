namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class ThirdPartyComponent
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public string Path { get; set; } = "";
    public string CommitId { get; set; } = "";
    public int VersionOrder
    {
        get
        {
            var parts = Version.Split('.').Select(p => int.TryParse(p, out var num) ? num : 0).ToArray();
            return parts.Length > 0 ? parts.Aggregate(0, (acc, p) => acc * 1000 + p) : 0;
        }
    }
}