using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign(description: "Search for components",
                           arguments: "<search arguments>",
                  disableProxyOutput: true,
                             example: "//Search components|components <search1> <search2>...")]
    public class ComponentCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var filter = Input.SingleArgument.ToLower();
            var allComponents = ObjectStorage.GetThirdPartyComponents();
            List<ThirdPartyComponent> filteredComponents;
            var inputBuffer = filter;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("➡ Type to filter results, press ENTER to select, BACKSPACE to delete, ESC to exit:");
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
            if (filteredComponents.Count > 0)
            {
                var selected = ListService.ListDialog("Choose a component to view details.", filteredComponents.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
                if (selected.Count <= 0) return Ok();

                var component = filteredComponents[selected.First().Key];
                WriteLine("");
                WriteCodeExample(component.Name, component.Version);
                ProjectSearch(component, detailedSearch: true);

                var searchQuestion = DialogService.YesNoDialog($"Do you want to do a search for all versions of {component.Name}?");
                if (searchQuestion) ProjectSearch(component, detailedSearch: false);
            }
            return Ok();
        }
        private void ProjectSearch(ThirdPartyComponent component, bool detailedSearch)
        {
            var devProjects = ObjectStorage.GetDevProjects().Where(dp => dp.Components.Any(c => c.Name == component.Name && (c.Version == component.Version || !detailedSearch))).ToList();
            var projects = ObjectStorage.GetProjects().Where(p => devProjects.Any(dp => dp.ProjectId == p.Id)).ToList();
            var repos = new List<Repository>();
            var teams = ObjectStorage.GetTeams();
            foreach (var projectRepos in projects.Select(project => ObjectStorage.GetRepositories().Where(r => r.ProjectId == project.Id))) repos.AddRange(projectRepos);
            PresentationManager.DisplayOrganisation(configuration.ThirdEyeAgent.OrganisationName, projects, repos, teams, devProjects);
        }
    }
}