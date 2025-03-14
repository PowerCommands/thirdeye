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
}