using System.Reflection;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign(description: "Search for components",
                           arguments: "<search arguments>",
                             options: "analyze",
                  disableProxyOutput: true,
                             example: "//Search components|component <search1> <search2>...")]
    public class ComponentCommand(string identifier, PowerCommandsConfiguration config) : ThirdEyeBaseCommando(identifier, config)
    {
        public override RunResult Run()
        {
            var filter = Input.SingleArgument.ToLower();

            if (HasOption("analyze")) return Analyze(filter);

            var allComponents = Storage.GetThirdPartyComponents();
            List<ThirdPartyComponent> filteredComponents;
            var inputBuffer = filter;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("➡ Type to filter results, press ENTER to select, BACKSPACE to delete, ESC to exit:");
                Console.Title = inputBuffer;
                filteredComponents = allComponents.Where(c => c.Name.ToLower().Contains(inputBuffer) || c.Version.ToLower().Contains(inputBuffer) || c.Path.ToLower().Contains(inputBuffer)).ToList();
                if (filteredComponents.Count == 0) Console.WriteLine("No matching result... (Press ESC to exit)");
                else
                {
                    foreach (var c in filteredComponents) Console.WriteLine($"{c.Name} {c.Version}");
                }
                Console.Write("\nPress enter to continue with all matching items. ");
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
            Console.Title = $"{ConfigurationGlobals.ApplicationName} {ReflectionService.Service.GetVersion(Assembly.GetExecutingAssembly())}";
            if (filteredComponents.Count > 0)
            {
                var selected = ListService.ListDialog("Choose a component to view details.", filteredComponents.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
                if (selected.Count <= 0) return Ok();

                var component = filteredComponents[selected.First().Key];
                WriteLine("");
                WriteCodeExample(component.Name, component.Version);
                ProjectSearch(component, detailedSearch: true);
            }
            return Ok();
        }

        private RunResult Analyze(string filter)
        {
            var workflow = new AnalyzeComponentWorkflow(this, Configuration);
            workflow.Run(Input.SingleArgument);
            return Ok();
        }
        
    }
}