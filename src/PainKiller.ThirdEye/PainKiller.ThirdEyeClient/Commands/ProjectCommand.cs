using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.DomainObjects;
using PainKiller.ThirdEyeClient.Managers.Workflows;

namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign(description: "Handle projects",
    arguments: [],
    examples: ["//List all projects", "project"])]
public class ProjectCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var filter = $"{input.Arguments.FirstOrDefault()}".ToLower();
        var allProjects = Storage.GetProjects();

        InteractiveFilter<Project>.Run(
            allProjects,
            (project, filterString) => project.Name.ToLower().Contains(filterString),
            (projects, selectedIndex) =>
            {
                Console.Clear();
                Console.WriteLine("Choose a project to view details (use arrow keys to navigate, Enter to select):");
                var projectList = projects.ToList();
                for (int i = 0; i < projectList.Count; i++)
                {
                    var prefix = i == selectedIndex ? "> " : "  ";
                    Console.WriteLine($"{prefix}{projectList[i].Name}");
                }
            },
            (selectedProject) =>
            {
                if (selectedProject != null)
                {
                    Writer.WriteLine();
                    var repo = Storage.GetRepositories().FirstOrDefault(r => r.RepositoryId == selectedProject.RepositoryId);
                    PresentationManager.DisplayProject(selectedProject, repo.Name);
                    var analyzeProjectQuery = DialogService.YesNoDialog($"Do you want to analyze {selectedProject.Name} for vulnerabilities?");
                    if (analyzeProjectQuery)
                    {
                        var analyzer = new AnalyzeProjectWorkflow(Writer, Configuration);
                        analyzer.Run(selectedProject.Name.ToLower());
                    }
                }
            }
        );

        return Ok();
    }
}


    
