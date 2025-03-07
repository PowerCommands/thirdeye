using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class PresentationManager(IConsoleWriter writer)
{
    public void DisplayRepository(string name, IEnumerable<DevProject> projects)
    {
        writer.WriteHeadLine($"\n📁 {name}");
        foreach (var devProject in projects)
        {
            writer.WriteHeadLine($"├── 🈁 {devProject.Name} {devProject.Sdk} {devProject.Language} {devProject.Framework}");
            foreach (var component in devProject.Components) {writer.WriteHeadLine($"│  │   ├── {component.Name} {component.Version}");}
        }
    }
}