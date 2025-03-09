using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Extensions;

public static class ThirdEyeExtensions
{
    public static List<Project> GetFilteredProjects(this IEnumerable<Project> projects, string[] projectNames) => projects.Where(p => projectNames.Contains(p.Name) || projectNames.First() == "*").ToList();
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
}