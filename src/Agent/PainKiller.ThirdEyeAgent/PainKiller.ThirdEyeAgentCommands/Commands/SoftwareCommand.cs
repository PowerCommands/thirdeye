using System.Reflection;
using PainKiller.PowerCommands.Core.Commands;
using PainKiller.ThirdEyeAgentCommands.Data;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Enums;
using PainKiller.ThirdEyeAgentCommands.Managers;
using PainKiller.ThirdEyeAgentCommands.Services;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Handle software",
                           arguments: "filename",
                             options: "analyze",
                  disableProxyOutput: true,
                             example: "//Show all stored software|software|//Upload a file|software myFile.txt")]
    [PowerCommandsToolbar(["[Enter] = Show all software","Filename = Upload new software file","(use cd command and tab to select a file or just enter the filename)"])]
    [PowerCommandPrivacy]
    public class SoftwareCommand(string identifier, PowerCommandsConfiguration configuration) : CdCommand(identifier, configuration)
    {
        public override RunResult Run()
        {
            if (HasOption("analyze")) return Analyze();

            var filter = string.Join(' ', Input.Arguments);
            var filename = string.Join(' ', Input.Arguments);
            if (File.Exists(filename))
            {
                filter = "";
                var data = File.ReadAllLines(filename);
                var softwareObject = new SoftwareObjects();
                foreach (var line in data)
                {
                    if(string.IsNullOrEmpty(line)) continue;
                    var software = new Software(line);
                    softwareObject.Softwares.Add(software);
                }
                softwareObject.LastUpdated = DateTime.Now;
                StorageService<SoftwareObjects>.Service.StoreObject(softwareObject);
                WriteSuccessLine($"software in {filename} has been stored in database.");
            }
            DisplaySoftware(filter);
            return Ok();
        }
        public void DisplaySoftware(string filter)
        {
            var softwareItems = StorageService<SoftwareObjects>.Service.GetObject().Softwares;
            List<Software> filteredSoftware;
            var inputBuffer = filter;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("➡ Type to filter results, press ENTER to select, BACKSPACE to delete, ESC to exit:");
                Console.Title = inputBuffer;
                filteredSoftware = softwareItems.Where(c => c.Name.ToLower().Contains(inputBuffer) || c.Version.ToLower().Contains(inputBuffer) || c.MetaInformation.ToLower().Contains(inputBuffer)).ToList();
                if (filteredSoftware.Count == 0) Console.WriteLine("No matching result... (Press ESC to exit)");
                else
                {
                    foreach (var c in filteredSoftware) Console.WriteLine($"{c.Name} {c.Version} {c.MetaInformation}");
                }
                Console.Write("\nPress enter to continue with all matching items. ");
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Escape) return;
                if (key.Key == ConsoleKey.Enter && filteredSoftware.Count > 0) break;
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
            if (filteredSoftware.Count > 0)
            {
                var selected = ListService.ListDialog("Choose a component to view details.", filteredSoftware.Select(c => $"{c.Name} Version: {c.Version} {c.MetaInformation}").ToList(), autoSelectIfOnlyOneItem: false);
                if (selected.Count <= 0) return;

                var component = filteredSoftware[selected.First().Key];
                WriteLine("");
                WriteCodeExample(component.Name, component.Version);
            }
        }

        private RunResult Analyze()
        {
            var cveStorage = CveStorageService.Service;
            var software = StorageService<SoftwareObjects>.Service.GetObject();
            var presentationManager = new PresentationManager(this);

            ConsoleService.Service.Clear();
            WriteHeadLine("Analyze begins, loading CVEs...");
            if(cveStorage.LoadedCveCount == 0) cveStorage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();

            var analyzer = new CveAnalyzeManager(this);
            var threshold = ToolbarService.NavigateToolbar<CvssSeverity>();
            var components = analyzer.GetVulnerabilities(cveStorage.GetCveEntries(), software.Softwares,threshold);
            var selectedComponentCves = presentationManager.DisplayVulnerableComponents(components);
            var selected = ListService.ListDialog("Choose a software to view details.", selectedComponentCves.Select(c => $"{c.Name} {c.Version}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selected.Count <= 0) return Ok();
            var component = selectedComponentCves[selected.First().Key];
            var cve = presentationManager.DisplayVulnerableComponent(component);

            return Ok();
        }
    }
}