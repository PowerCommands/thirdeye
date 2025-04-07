using System.Text.Json;
using System.Xml;
using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Managers;

public static class ProjectManager
{
    private static readonly ILogger<Program> _logger = LoggerProvider.CreateLogger<Program>();
    public static List<Project> IdentifyProjects(List<Item> files)
    {
        try
        {
            var projectFiles = files.Where(f => f.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || f.Path.EndsWith("vbproj.json", StringComparison.OrdinalIgnoreCase) || f.Path.EndsWith("package.json", StringComparison.OrdinalIgnoreCase) || f.Path.EndsWith("go.mod", StringComparison.OrdinalIgnoreCase) || f.Path.EndsWith("pom.xml", StringComparison.OrdinalIgnoreCase)).ToList();
            return projectFiles.Select(file => new Project { Name = Path.GetFileNameWithoutExtension(file.Path), Path = Path.GetDirectoryName(file.Path) ?? "", Version = ExtractVersion(file), Framework = ExtractFramework(file), Language = DetectLanguage(files), Sdk = ExtractSdk(file)}).ToList();
        }
        catch(Exception ex)
        {
            _logger.LogError($"{nameof(ProjectManager)} {nameof(IdentifyProjects)} {ex.Message}");
        }
        return [];
    }
    private static string ExtractVersion(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) return "-";
        try
        {
            if (file.Path.EndsWith("package.json", StringComparison.OrdinalIgnoreCase))
            {
                var json = JsonDocument.Parse(file.Content);
                if (json.RootElement.TryGetProperty("version", out var versionElement))
                {
                    return versionElement.GetString() ?? "-";
                }
            }
            else if (file.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(file.Content);
                var versionNode = xmlDoc.SelectSingleNode("//Version");
                return versionNode?.InnerText ?? "-";
            }
        }
        catch(Exception ex)
        {
            _logger.LogError($"{nameof(ProjectManager)} {nameof(ExtractVersion)} {ex.Message}");
        }
        return "-";
    }
    private static string ExtractFramework(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) return "-";
        try
        {
            if (file.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || file.Path.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(file.Content);
                var frameworkNode = xmlDoc.SelectSingleNode("//TargetFramework") ?? xmlDoc.SelectSingleNode("//TargetFrameworks");
                return frameworkNode?.InnerText ?? "-";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ProjectManager)} {nameof(ExtractFramework)} {ex.Message}");
        }
        return "-";
    }

    private static string ExtractSdk(Item file)
    {
        if (string.IsNullOrEmpty(file.Content)) return "-";
        try
        {
            if (file.Path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || file.Path.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(file.Content);
                var sdkNode = xmlDoc.DocumentElement?.Attributes["Sdk"];
                var sdk = sdkNode?.Value ?? "-";
                return sdk;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ProjectManager)} {nameof(ExtractSdk)} {ex.Message}");
        }
        return "-";
    }
    private static string DetectLanguage(IEnumerable<Item> files)
    {
        try
        {
            var primaryLanguages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".cs", "C#" },
                { ".vb", "Visual Basic" },
                { ".java", "Java" },
                { ".ts", "TypeScript" },
                { ".py", "Python" },
                { ".r", "R" }
            };
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.Path);
                if (primaryLanguages.TryGetValue(extension, out var language)) return language;
            
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ProjectManager)} {nameof(DetectLanguage)} {ex.Message}");
        }
        return "-";
    }
}

