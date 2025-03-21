using System.Reflection;
using PainKiller.PowerCommands.Shared.Enums;
using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle repositories",
                  disableProxyOutput: true,
                             example: "//List all repositories|repository")]
    public class RepositoryCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            DisableLog();
            var filter = Input.SingleArgument;
            var allRepositories = Storage.GetRepositories();

            List<Repository> filtered;
            var inputBuffer = filter;
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{Emo.Right.Icon()} Type to filter results, press ENTER {Emo.Enter.Icon()} to select, BACKSPACE {Emo.Backspace.Icon()} to delete, ESC {Emo.Escape.Icon()} to exit:");
                Console.Title = inputBuffer;
                filtered = allRepositories.Where(p => p.Name.ToLower().Contains(inputBuffer)).ToList();
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
                var selected = ListService.ListDialog("Choose a repository to view details.", filtered.Select(p => $"{p.Name}").ToList(), autoSelectIfOnlyOneItem: false);
                if (selected.Count <= 0) return Ok();
                var repository = filtered[selected.First().Key];
                
                WriteLine("");
                var projects = Storage.GetProjects().Where(p => p.RepositoryId == repository.RepositoryId).ToList();
                PresentationManager.DisplayRepository(repository.Name, projects);
                var analyzeProjectQuery = DialogService.YesNoDialog($"Do you want to analyze {repository.Name} for vulnerabilities?");
                if (analyzeProjectQuery)
                {
                    var analyzer = new AnalyzeRepositoryWorkflow(this, Configuration);
                    analyzer.Run(repository.Name);
                }
            }
            EnableLog();
            return Ok();
        }
    }
}