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

        List<Project> filtered;
        var inputBuffer = filter;
        while (true)
        {
            Writer.Clear();
            Writer.WriteLine("➡ Type to filter results, press ENTER to select, BACKSPACE to delete, ESC to exit:");
            Writer.WriteLine($"Current filter: {inputBuffer}");
            filtered = allProjects.Where(p => p.Name.ToLower().Contains(inputBuffer)).ToList();

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

        Writer.WriteLine();
        Writer.WriteLine($"Project selection completed with {filtered.Count} items.");

        Writer.WriteLine($"{Configuration.Core.Name} {Configuration.Core.Version}");
        
        if (filtered.Count > 0)
        {
            var selected = ListService.ListDialog("Choose a project to view details.", filtered.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selected.Count <= 0) return Ok();

            var project = filtered[selected.First().Key];
            Writer.WriteLine();
            PresentationManager.DisplayProject(project);

            var analyzeProjectQuery = DialogService.YesNoDialog($"Do you want to analyze {project.Name} for vulnerabilities?");
            if (analyzeProjectQuery)
            {
                var analyzer = new AnalyzeProjectWorkflow(Writer, Configuration);
                analyzer.Run(project.Name.ToLower());
            }
        }
        return Ok();
    }
}

    
