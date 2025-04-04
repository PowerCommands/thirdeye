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

[CommandDesign(description: "Search for components, analyze them for vulnerabilities.",
    arguments: ["<search arguments>"],
    options: ["analyze"],
    examples: ["//Search components","component <search1> <search2>...","//Analyze all components","component --analyze","//Analyze with filter","component <search> --analyze"])]
public class ComponentCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var filter = $"{input.Arguments.FirstOrDefault()}".ToLower();

        if (input.HasOption("analyze")) return Analyze(filter);

        var allComponents = Storage.GetThirdPartyComponents();
        List<ThirdPartyComponent> filteredComponents;
        var inputBuffer = filter;
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"{Emo.Right.Icon()} Type to filter results, press ENTER {Emo.Enter.Icon()} to select, BACKSPACE {Emo.Backspace.Icon()}  to delete, ESC {Emo.Escape.Icon()} to exit:");
            Console.Title = inputBuffer;
            filteredComponents = allComponents.Where(c => c.Name.ToLower().Contains(inputBuffer) || c.Version.ToLower().Contains(inputBuffer) || c.Path.ToLower().Contains(inputBuffer)).ToList();
            if (filteredComponents.Count == 0) Console.WriteLine($"No matching result... (Press ESC {Emo.Escape.Icon()} to exit)");
            else
            {
                foreach (var c in filteredComponents) Console.WriteLine($"{c.Name} {c.Version}");
            }
            Console.Write($"\nPress enter {Emo.Enter.Icon()} to continue with all matching items. ");
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape) return Ok();
            if (key.Key == ConsoleKey.Enter && filteredComponents.Count > 0) break;
            if (key.Key == ConsoleKey.Backspace)
            {
                if (inputBuffer.Length > 0) inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1);
            }
            else if (!char.IsControl(key.KeyChar))
            {
                inputBuffer += key.KeyChar;
            }
        }
        Console.Title = $"{Configuration.Core.Name} {Configuration.Core.Version}";
        if (filteredComponents.Count > 0)
        {
            var selected = ListService.ListDialog("Choose a component to view details.", filteredComponents.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selected.Count <= 0) return Ok();

            var component = filteredComponents[selected.First().Key];
            Writer.WriteLine();
            Writer.WriteDescription(component.Name, component.Version);
            var analyzeComponent = DialogService.YesNoDialog("Do you want to analyze this component?");
            if (analyzeComponent)
            {
                var workflow = new AnalyzeComponentWorkflow(this.Writer, Configuration, component);
                workflow.Run(filter);
                return Ok();
            }
            ProjectSearch(component, detailedSearch: true);
        }
        return Ok();
    }
    private RunResult Analyze(string filter)
    {
        var workflow = new AnalyzeComponentWorkflow(this.Writer, Configuration, new ThirdPartyComponent{Name = "*"});
        workflow.Run(filter);
        return Ok();
    }
}