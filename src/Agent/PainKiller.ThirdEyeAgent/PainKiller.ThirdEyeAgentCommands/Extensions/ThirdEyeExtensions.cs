using System.Security.Cryptography;
using System.Text;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;

namespace PainKiller.ThirdEyeAgentCommands.Extensions;

public static class ThirdEyeExtensions
{
    public static List<Workspace> GetFilteredWorkspaces(this IEnumerable<Workspace> workspaces, string[] workspaceNames) => workspaces.Where(p => workspaceNames.Contains(p.Name) || workspaceNames.First() == "*").ToList();
    public static Guid ToGuid(this long value)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

    public static IEnumerable<T> ConfigurationFilter<T>(this ThirdEyeConfiguration configuration, List<T> items, IEnumerable<string> filters, Func<T, string> keySelector) where T : new()
    {
        var retVal = new List<T>();
        if(items.Count == 0) return retVal;
        foreach (var filter in filters)
        {
            if (filter == "*")
            {
                retVal.AddRange(items);
                break;
            }
            var foundItem = items.FirstOrDefault(p => string.Equals(keySelector(p), filter, StringComparison.CurrentCultureIgnoreCase));
            if (foundItem != null)
            {
                retVal.Add(foundItem);
            }
        }
        return retVal;
    }
    public static CvssSeverity GetSeverity(this float score)
    {
        if (score >= 9.0) return CvssSeverity.Critical;
        if (score >= 7.0) return CvssSeverity.High;
        if (score >= 4.0) return CvssSeverity.Medium;
        if (score >= 0.1) return CvssSeverity.Low;
        return CvssSeverity.None;
    }
    public static float GetScore(this CvssSeverity severity)
    {
        return severity switch
        {
            CvssSeverity.Critical => 9.0f,
            CvssSeverity.High => 7.0f,
            CvssSeverity.Medium => 4.0f,
            CvssSeverity.Low => 0.1f,
            _ => 0.0f,
        };
    }
    public static float GetScoreFromString(this string severity)
    {
        if (string.IsNullOrWhiteSpace(severity)) return 0.0f;
    
        return severity.ToLower() switch
        {
            "critical" => 9.0f,
            "high" => 7.0f,
            "medium" => 4.0f,
            "low" => 0.1f,
            _ => 0.0f,
        };
    }
    public static string GetDisplaySeverity(this float score)
    {
        return score.GetSeverity() switch
        {
            CvssSeverity.Critical => "🔴 Critical",
            CvssSeverity.High => "🟠 High",
            CvssSeverity.Medium => "🟡 Medium",
            CvssSeverity.Low => "🟢 Low",
            _ => "⚪ None"
        };
    }
    public static bool IsEqualOrHigher(this float score, CvssSeverity threshold)
    {
        return score.GetSeverity() >= threshold;
    }
    public static int ToVersionOrder(this ComponentCve cve)
    {
        var parts = cve.Version.Split('.').Select(p => int.TryParse(p, out var num) ? num : 0).ToArray();
        return parts.Length > 0 ? parts.Aggregate(0, (acc, p) => acc * 1000 + p) : 0;
    }
    public static GitHostType GetGitHostType(this string host)
    {
        if (host.Contains("github.com")) return GitHostType.Github;
        if (Uri.TryCreate(host, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) return GitHostType.Ads;
        return GitHostType.Local;
    }
    public static int ToVersionOrder(this string version)
    {
        var parts = version.Split('.').Select(p => int.TryParse(p, out var num) ? num : 0).ToArray();
        return parts.Length > 0 ? parts.Aggregate(0, (acc, p) => acc * 1000 + p) : 0;
    }
    public static string GenerateSignature(this IEnumerable<ThirdPartyComponent> components, CvssSeverity severity)
    {
        var orderedComponents = components
            .OrderBy(c => c.Name)
            .ThenBy(c => c.VersionOrder)
            .Select(c => $"{c.Name}-{c.Version}")
            .ToList();

        var inputString = string.Join("|", orderedComponents) + $"|Severity:{(int)severity}";
        return ComputeSha256Hash(inputString);
    }
    private static string ComputeSha256Hash(string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}