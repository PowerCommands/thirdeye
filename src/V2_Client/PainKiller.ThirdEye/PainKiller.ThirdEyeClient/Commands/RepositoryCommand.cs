using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Enums;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ThirdEyeClient.BaseClasses;
using PainKiller.ThirdEyeClient.DomainObjects;
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

        List<Repository> filtered;
        var inputBuffer = filter;
        while (true)
        {
            Writer.Clear();
            Writer.WriteLine($"{Emo.Right.Icon()} Type to filter results, press ENTER {Emo.Enter.Icon()} to select, BACKSPACE {Emo.Backspace.Icon()} to delete, ESC {Emo.Escape.Icon()} to exit:");
            Writer.WriteLine($"Current filter: {inputBuffer}");
            filtered = allRepositories.Where(p => p.Name.ToLower().Contains(inputBuffer)).ToList();

            if (filtered.Count == 0)
            {
                Writer.WriteLine("No matching result... (Press ESC to exit)");
            }
            else
            {
                foreach (var p in filtered)
                {
                    Writer.WriteLine($"{p.Name}");
                }
            }

            Writer.WriteLine("\nPress enter to continue with all matching items.");
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape) return Ok();
            if (key.Key == ConsoleKey.Enter && filtered.Count > 0) break;
            if (key.Key == ConsoleKey.Backspace)
            {
                if (inputBuffer.Length > 0) inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1);
            }
            else if (!char.IsControl(key.KeyChar))
            {
                inputBuffer += key.KeyChar;
            }
        }

        Writer.WriteLine("");
        Writer.WriteLine($"{Configuration.Core.Name} {Configuration.Core.Version}");
        if (filtered.Count > 0)
        {
            var selected = ListService.ListDialog("Choose a repository to view details.", filtered.Select(p => $"{p.Name}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selected.Count <= 0) return Ok();

            var repository = filtered[selected.First().Key];
            Writer.WriteLine("");
            var projects = Storage.GetProjects().Where(p => p.RepositoryId == repository.RepositoryId).ToList();
            PresentationManager.DisplayRepository(repository.Name, projects);

            var analyzeProjectQuery = DialogService.YesNoDialog($"Do you want to analyze {repository.Name} for vulnerabilities?");
            if (analyzeProjectQuery)
            {
                var analyzer = new AnalyzeRepositoryWorkflow(Writer, Configuration);
                analyzer.Run(repository.Name);
            }
        }
        return Ok();
    }
}