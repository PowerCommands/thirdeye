using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
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

        InteractiveFilter<ThirdPartyComponent>.Run(
            allComponents,
            (component, filterString) => component.Name.ToLower().Contains(filterString) ||
                                         component.Version.ToLower().Contains(filterString) ||
                                         component.Path.ToLower().Contains(filterString),
            (components, selectedIndex) =>
            {
                Console.Clear();
                Console.WriteLine("Choose a component to view details (use arrow keys to navigate, Enter to select):");
                var thirdPartyComponents = components.ToList();
                for (int i = 0; i < thirdPartyComponents.Count; i++)
                {
                    var prefix = i == selectedIndex ? "> " : "  ";
                    Console.WriteLine($"{prefix}{thirdPartyComponents[i].Name} {thirdPartyComponents[i].Version}");
                }
            },
            (selectedComponent) =>
            {
                if (selectedComponent != null)
                {
                    Writer.WriteLine();
                    Writer.WriteDescription(selectedComponent.Name, selectedComponent.Version);
                    var analyzeComponent = DialogService.YesNoDialog("Do you want to analyze this component?");
                    if (analyzeComponent)
                    {
                        var workflow = new AnalyzeComponentWorkflow(this.Writer, Configuration, selectedComponent);
                        workflow.Run(filter);
                    }
                    else
                    {
                        ProjectSearch(selectedComponent, detailedSearch: true);
                    }
                }
            }
        );

        return Ok();
    }

    private RunResult Analyze(string filter)
    {
        var workflow = new AnalyzeComponentWorkflow(this.Writer, Configuration, new ThirdPartyComponent{Name = "*"});
        workflow.Run(filter);
        return Ok();
    }
}
