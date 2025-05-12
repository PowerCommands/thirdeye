using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Managers;

public class ReportManager(string fileName = "report.md")
{
    public void Create(string organizationName, List<Repository> repositories, List<Project> projects, ThirdPartyComponent filter)
    {
        var mdDoc = new MarkdownManager();
        mdDoc.AddParagraph($"Component found in {repositories.Count} repositories.");
        foreach (var repository in repositories)
        {
            mdDoc.AddHeader(repository.Name, 1);
            var filteredProjects = projects.Where(p => p.RepositoryId == repository.RepositoryId).ToList();
            foreach (var project in filteredProjects)
            {
                if (filter != null && project.Components.All(c => c.Name != filter.Name)) continue;
                mdDoc.AddHeader(project.Name, 2);
                if (project.Components.Count > 0)
                {
                    var components = project.Components.Where(c => c.Name.Trim().ToLower() == filter.Name.Trim().ToLower()).Select(c => $"{c.Name} {c.Version}").ToList();
                    mdDoc.AddList(components);
                }
            }
        }
        mdDoc.SaveToFile($"{fileName.FormatFileTimestamp()}.md");
    }
}