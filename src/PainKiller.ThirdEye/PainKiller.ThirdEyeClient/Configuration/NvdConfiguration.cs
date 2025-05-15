namespace PainKiller.ThirdEyeClient.Configuration;

public class NvdConfiguration
{
    private string _pathToUpdates = "updates";
    private string _path = "nvd";
    public string Url { get; set; } = "https://services.nvd.nist.gov/rest/json/cves/2.0";
    public int TimeoutSeconds { get; set; } = 120;
    public int PageSize { get; set; } = 2000;
    public int DelayIntervalSeconds { get; set; } = 10;
    public string PathToUpdates
    {
        get => _pathToUpdates.GetReplacedPlaceHolderPath();
        set => _pathToUpdates = value;
    }
    public string Path
    {
        get => _path.GetReplacedPlaceHolderPath();
        set => _path = value;
    }
    public int LatestCount { get; set; } = 100;
    public string TokenName { get; set; } = "TE_api_key";
}