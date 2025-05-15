namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign(description: "Search for components",

    arguments: ["<search arguments>"],

    examples: ["//Search after components with the name hangfire", "search hangfire"])]

public class SearchCommand(string identifier) : ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var initFilter = $"{input.Arguments.FirstOrDefault()}".ToLower();
        var allComponents = Storage.GetThirdPartyComponents();
        InteractiveFilter<ThirdPartyComponent>.Run(allComponents, (component, filterString) => component.Name.ToLower().Contains(filterString) || component.Version.ToLower().Contains(filterString), (components, selectedIndex) =>
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
                    ComponentSearch(selectedComponent, detailedSearch: false);
                }
            }, initFilter
        );
        return Ok();
    }
}