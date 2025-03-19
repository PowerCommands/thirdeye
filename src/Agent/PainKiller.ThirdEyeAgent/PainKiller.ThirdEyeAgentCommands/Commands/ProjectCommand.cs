using System.Reflection;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle projects",
                  disableProxyOutput: true,
                             example: "//List all projects|project")]
    public class ProjectCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var filter = Input.SingleArgument;
            var allProjects = Storage.GetProjects();

            List<Project> filtered;
            var inputBuffer = filter;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("➡ Type to filter results, press ENTER to select, BACKSPACE to delete, ESC to exit:");
                Console.Title = inputBuffer;
                filtered = allProjects.Where(p => p.Name.ToLower().Contains(inputBuffer)).ToList();
                if (filtered.Count == 0) Console.WriteLine("No matching result... (Press ESC to exit)");
                else
                {
                    foreach (var p in filtered) Console.WriteLine($"{p.Name}");
                }
                Console.Write("\nPress enter to continue with all matching items. ");
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
            Console.Title = $"{ConfigurationGlobals.ApplicationName} {ReflectionService.Service.GetVersion(Assembly.GetExecutingAssembly())}";
            if (filtered.Count > 0)
            {
                var selected = ListService.ListDialog("Choose a project to view details.", filtered.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
                if (selected.Count <= 0) return Ok();
                var project = filtered[selected.First().Key];
                
                WriteLine("");
                PresentationManager.DisplayProject(project);
                var analyzeProjectQuery = DialogService.YesNoDialog($"Do you want to analyze {project.Name} for vulnerabilities?");
                if (analyzeProjectQuery)
                {
                    var analyzer = new AnalyzeProjectWorkflow(this, Configuration);
                    analyzer.Run(project.Name.ToLower());
                }
            }
            EnableLog();
            return Ok();
        }
    }
}