namespace PainKiller.ThirdEyeClient.DomainObjects.Nvd;

public class CveEntry
{
    public string Id { get; set; } = "";
    public string Description { get; set;} = "";
    public float CvssScore { get; set; } 
    public string Severity { get; set; } = "";
    public List<string> AffectedProducts { get; set; } = [];

    public bool IsAffectedProduct(string componentName, string componentVersion)
    {
        return (AffectedProducts.Any(p => !string.IsNullOrEmpty(p) && p.ToLower().Contains(componentName.ToLower())));
    }
    public int FetchedIndex { get; set; }
}