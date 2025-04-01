using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Core.Enums;
using PainKiller.CommandPrompt.CoreLib.Core.Events;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Core.Presentation;
using PainKiller.CommandPrompt.CoreLib.Core.Services;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ReadLine.Managers;
using PainKiller.ThirdEyeClient.Bootstrap;
using PainKiller.ThirdEyeClient.Data;
using PainKiller.ThirdEyeClient.DomainObjects;
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
        List<Software> filteredSoftware;
        var inputBuffer = filter;
        while (true)
        {
            Writer.Clear();
            Writer.WriteLine($"{Emo.Right.Icon()} Type to filter results, press ENTER {Emo.Enter.Icon()} to select, BACKSPACE {Emo.Backspace.Icon()} to delete, ESC {Emo.Escape.Icon()} to exit:");
            Writer.WriteLine($"Current filter: {inputBuffer}");
            filteredSoftware = softwareItems.Where(c => 
                c.Name.ToLower().Contains(inputBuffer) || 
                c.Version.ToLower().Contains(inputBuffer) || 
                c.MetaInformation.ToLower().Contains(inputBuffer)
            ).ToList();

            if (filteredSoftware.Count == 0) 
                Writer.WriteLine($"No matching result... (Press Escape {Emo.Escape.Icon()} to exit)");
            else
            {
                foreach (var c in filteredSoftware) 
                    Writer.WriteLine($"{c.Name} {c.Version} {c.MetaInformation}");
            }

            Writer.WriteLine($"\nPress enter {Emo.Enter.Icon()} to continue with all matching items.");
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

        Writer.WriteLine();
        Writer.WriteLine($"{Configuration.Core.Name} {Configuration.Core.Version}");
        
        if (filteredSoftware.Count > 0)
        {
            var selected = ListService.ListDialog("Choose a component to view details.", filteredSoftware.Select(c => $"{c.Name} Version: {c.Version} {c.MetaInformation}").ToList(), autoSelectIfOnlyOneItem: false);
            if (selected.Count <= 0) return;

            var component = filteredSoftware[selected.First().Key];
            Writer.WriteLine();
            Writer.WriteDescription(component.Name, component.Version);
        }
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