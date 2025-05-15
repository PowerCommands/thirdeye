using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Events;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ReadLine.Managers;
using PainKiller.ThirdEyeClient.Configuration;
using PainKiller.ThirdEyeClient.Data;
using PainKiller.ThirdEyeClient.Managers.Workflows;

namespace PainKiller.ThirdEyeClient.Commands;
[CommandDesign(description: "Handle software",
    arguments: ["filename"],
    options: ["analyze"],
    examples: ["//Show all stored software", "software", "//Upload a file", "software myFile.txt", "//Analyze all softwares", "software --analyze", "//Show a previous analyze", "analyze myFile.txt --analyze"])]
public class SoftwareCommand : ConsoleCommandBase<CommandPromptConfiguration>
{
    private void OnWorkingDirectoryChanged(WorkingDirectoryChangedEventArgs e) => UpdateSuggestions(e.NewWorkingDirectory);
    public SoftwareCommand(string identifier) : base(identifier) => EventBusService.Service.Subscribe<WorkingDirectoryChangedEventArgs>(OnWorkingDirectoryChanged);

    public override RunResult Run(ICommandLineInput input)
    {
        var fileName = string.Join(' ', input.Arguments);
        if (input.HasOption("analyze")) return Analyze(fileName);

        var filter = string.Join(' ', input.Arguments);
        if (File.Exists(fileName))
        {
            filter = "";
            var data = File.ReadAllLines(fileName);
            var softwareObject = new SoftwareObjects();
            foreach (var line in data)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var software = new Software(line);
                softwareObject.Items.Add(software);
            }
            softwareObject.LastUpdated = DateTime.Now;
            StorageService<SoftwareObjects>.Service.StoreObject(softwareObject);
            Writer.WriteSuccessLine($"Software in {fileName} has been stored in database.");
        }
        DisplaySoftware(filter);
        return Ok();
    }

    public void DisplaySoftware(string filter)
    {
        var softwareItems = StorageService<SoftwareObjects>.Service.GetObject().Items;

        InteractiveFilter<Software>.Run(
            softwareItems,
            (software, filterString) => software.Name.ToLower().Contains(filterString) ||
                                         software.Version.ToLower().Contains(filterString) ||
                                         software.MetaInformation.ToLower().Contains(filterString),
            (softwares, selectedIndex) =>
            {
                Console.Clear();
                Console.WriteLine("Choose a software item to view details (use arrow keys to navigate, Enter to select):");
                var softwareList = softwares.ToList();
                for (int i = 0; i < softwareList.Count; i++)
                {
                    var prefix = i == selectedIndex ? "> " : "  ";
                    Console.WriteLine($"{prefix}{softwareList[i].Name} Version: {softwareList[i].Version} {softwareList[i].MetaInformation}");
                }
            },
            (selectedSoftware) =>
            {
                if (selectedSoftware != null)
                {
                    Writer.WriteLine();
                    Writer.WriteDescription(selectedSoftware.Name, selectedSoftware.Version);
                }
            }
        );
    }
    private RunResult Analyze(string fileName)
    {
        var workflow = new AnalyzeSoftwareWorkflow(Writer, Configuration);
        workflow.Run(fileName);
        return Ok();
    }
    private void UpdateSuggestions(string newWorkingDirectory)
    {
        if (Directory.Exists(newWorkingDirectory))
        {
            var files = Directory.GetFiles(newWorkingDirectory)
                .Select(d => new DirectoryInfo(d).Name)
                .ToArray();
            SuggestionProviderManager.AppendContextBoundSuggestions(Identifier, files.Select(e => e).ToArray());
        }
    }
}
