using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Extensions;

public static class ThirdEyeExtensions
{
    public static List<Project> GetFilteredProjects(this IEnumerable<Project> projects, string[] projectNames) => projects.Where(p => projectNames.Contains(p.Name)).ToList();
}