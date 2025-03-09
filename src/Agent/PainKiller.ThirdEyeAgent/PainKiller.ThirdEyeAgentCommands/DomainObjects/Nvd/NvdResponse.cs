namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.Nvd;


public class Rootobject
{
    public int resultsPerPage { get; set; }
    public int startIndex { get; set; }
    public int totalResults { get; set; }
    public string format { get; set; }
    public string version { get; set; }
    public DateTime timestamp { get; set; }
    public Vulnerability[] vulnerabilities { get; set; }
}

public class Vulnerability
{
    public Cve cve { get; set; }
}

public class Cve
{
    public string id { get; set; }
    public string sourceIdentifier { get; set; }
    public DateTime published { get; set; }
    public DateTime lastModified { get; set; }
    public string vulnStatus { get; set; }
    public object[] cveTags { get; set; }
    public Description[] descriptions { get; set; }
    public Metrics metrics { get; set; }
    public Weakness[] weaknesses { get; set; }
    public Configuration[] configurations { get; set; }
    public Reference[] references { get; set; }
}

public class Metrics
{
    public Cvssmetricv2[] cvssMetricV2 { get; set; }
}

public class Cvssmetricv2
{
    public string source { get; set; }
    public string type { get; set; }
    public Cvssdata cvssData { get; set; }
    public string baseSeverity { get; set; }
    public float exploitabilityScore { get; set; }
    public float impactScore { get; set; }
    public bool acInsufInfo { get; set; }
    public bool obtainAllPrivilege { get; set; }
    public bool obtainUserPrivilege { get; set; }
    public bool obtainOtherPrivilege { get; set; }
    public bool userInteractionRequired { get; set; }
}

public class Cvssdata
{
    public string version { get; set; }
    public string vectorString { get; set; }
    public float baseScore { get; set; }
    public string accessVector { get; set; }
    public string accessComplexity { get; set; }
    public string authentication { get; set; }
    public string confidentialityImpact { get; set; }
    public string integrityImpact { get; set; }
    public string availabilityImpact { get; set; }
}

public class Description
{
    public string lang { get; set; }
    public string value { get; set; }
}

public class Weakness
{
    public string source { get; set; }
    public string type { get; set; }
    public Description1[] description { get; set; }
}

public class Description1
{
    public string lang { get; set; }
    public string value { get; set; }
}

public class Configuration
{
    public Node[] nodes { get; set; }
}

public class Node
{
    public string _operator { get; set; }
    public bool negate { get; set; }
    public Cpematch[] cpeMatch { get; set; }
}

public class Cpematch
{
    public bool vulnerable { get; set; }
    public string criteria { get; set; }
    public string matchCriteriaId { get; set; }
}

public class Reference
{
    public string url { get; set; }
    public string source { get; set; }
}

public class CveEntry
{
    public string Id { get; set; }
    public string Description { get; set; }
    public float CvssScore { get; set; }
    public string Severity { get; set; }
    public List<string> AffectedProducts { get; set; }
}
