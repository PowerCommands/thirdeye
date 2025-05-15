using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.Managers.Workflows;

namespace PainKiller.ThirdEyeClient.Commands;
[CommandDesign(description: "Handle repositories",
    arguments: [],
    examples: ["//List all repositories", "repository"])]
public class RepositoryCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var filter = $"{input.Arguments.FirstOrDefault()}".ToLower();
        var allRepositories = Storage.GetRepositories();

        InteractiveFilter<Repository>.Run(
            allRepositories,
            (repository, filterString) => repository.Name.ToLower().Contains(filterString),
            (repositories, selectedIndex) =>
            {
                Console.Clear();
                Console.WriteLine("Choose a repository to view details (use arrow keys to navigate, Enter to select):");
                var repoList = repositories.ToList();
                for (int i = 0; i < repoList.Count; i++)
                {
                    var prefix = i == selectedIndex ? "> " : "  ";
                    Console.WriteLine($"{prefix}{repoList[i].Name}");
                }
            },
            (selectedRepository) =>
            {
                if (selectedRepository != null)
                {
                    Writer.WriteLine("");
                    var projects = Storage.GetProjects().Where(p => p.RepositoryId == selectedRepository.RepositoryId).ToList();
                    PresentationManager.DisplayRepository(selectedRepository.Name, projects);

                    var analyzeProjectQuery = DialogService.YesNoDialog($"Do you want to analyze {selectedRepository.Name} for vulnerabilities?");
                    if (analyzeProjectQuery)
                    {
                        var analyzer = new AnalyzeRepositoryWorkflow(Writer, Configuration);
                        analyzer.Run(selectedRepository.Name);
                    }
                }
            }
        );

        return Ok();
    }
}
