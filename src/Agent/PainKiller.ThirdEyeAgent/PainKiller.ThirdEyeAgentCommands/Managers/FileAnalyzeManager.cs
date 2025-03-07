using PainKiller.ThirdEyeAgentCommands.DomainObjects;
using PainKiller.ThirdEyeAgentCommands.Managers.ComponentExtractors;

namespace PainKiller.ThirdEyeAgentCommands.Managers;

public class FileAnalyzeManager
{
    public static bool IsRelevantFile(string path)
    {
        var relevantExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".csproj",      // .NET 
            "packages.config", // NuGet
            "global.json",  // .NET SDK version
            "Directory.Build.props", // dependencies .NET-projekt
            "Directory.Build.targets",
            "package.json", // Node.js/NPM
            "yarn.lock",
            "pnpm-lock.yaml",
            "requirements.txt", // Python pip
            "Pipfile",
            "pyproject.toml",
            "composer.json", // PHP Composer
            "Gemfile", // Ruby Gems
            "pom.xml", // Java Maven
            "build.gradle", // Java Gradle
            "build.gradle.kts",
            "go.mod", // Go Modules
            "go.sum",
            "Cargo.toml", // Rust Cargo
            "Cargo.lock",
            "mix.exs", // Elixir Mix
        };
        return relevantExtensions.Contains(Path.GetExtension(path));
    }
    private static List<Item> GetRelevantFiles(IEnumerable<Item> files) => files.Where(file => !file.IsFolder && IsRelevantFile(file.Path)).ToList();
    public static List<ThirdPartyComponent> AnalyzeFiles(IEnumerable<Item> files)
    {
        var retVal = new List<ThirdPartyComponent>();
        var relevantFiles = GetRelevantFiles(files);
        var extractor = new CsProjExtractor();
        foreach (var file in relevantFiles)
        {
            if (extractor.CanHandle(file))
            {
                retVal.AddRange(extractor.ExtractComponents(file));
            }
        }
        return retVal;
    }
}