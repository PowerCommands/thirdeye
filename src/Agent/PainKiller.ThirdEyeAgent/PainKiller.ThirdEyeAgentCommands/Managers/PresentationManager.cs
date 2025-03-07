using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class PresentationManager(IConsoleWriter writer)
{
    public void DisplayRepository(string name, List<Item> repositoryItems, Analyze analyze)
    {
        writer.WriteHeadLine($"\n📁 {name}");
        foreach (var devProject in analyze.DevProjects)
        {
            writer.WriteHeadLine($"├── 🈁 {devProject.Name} {devProject.Sdk} {devProject.Language} {devProject.Framework}");
            foreach (var component in devProject.Components) {writer.WriteHeadLine($"│  │   ├── {component.Name} {component.Version}");}
        }
    }
}