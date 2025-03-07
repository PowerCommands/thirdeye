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
}