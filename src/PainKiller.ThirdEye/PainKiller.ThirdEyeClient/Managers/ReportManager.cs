namespace PainKiller.ThirdEyeClient.Managers;

public class ReportManager(string fileName = "report.md")
{
    public string Create(string organizationName, List<Repository> repositories, List<Project> projects, string filter)
    {
        var mdDoc = new MarkdownManager();
        mdDoc.AddParagraph($"Component found in {repositories.Count} repositories.");
        foreach (var repository in repositories)
        {
            mdDoc.AddHeader(repository.Name, 1);
            var filteredProjects = projects.Where(p => p.RepositoryId == repository.RepositoryId).ToList();
            foreach (var project in filteredProjects)
            {
                if (filter != null && project.Components.All(c => c.Name != filter)) continue;
                mdDoc.AddHeader(project.Name, 2);
                if (project.Components.Count > 0)
                {
                    var components = project.Components.Where(c => c.Name.Trim().ToLower() == filter?.Trim().ToLower()).ToList();
                    foreach (var component in components)
                    {
                        mdDoc.AddBoldText($"Name:");
                        mdDoc.AddParagraph(component.Name);
                        var member = MemberManager.GetMemberAndTeams(component.UserId);
                        mdDoc.AddParagraph(member.Member.Name);

                    }
                    mdDoc.AddList(components.Select(c => $"{c.Name} {c.Version}"));
                }
            }
        }

        var fullName = $"{fileName.FormatFileTimestamp()}.md";
        mdDoc.SaveToFile(fullName);
        return fullName;
    }
}