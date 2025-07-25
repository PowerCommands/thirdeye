﻿using System.Reflection;
using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Managers;

public class FileAnalyzeManager : IFileAnalyzeManager
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
            "package-lock.json", // Node.js/NPM
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
        return relevantExtensions.Any(extension => path.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
    }
    private List<Item> GetRelevantFiles(IEnumerable<Item> files) => files.Where(file => !file.IsFolder && IsRelevantFile(file.Path)).ToList();
    public  FileAnalyze AnalyzeRepo(List<Item> repoItems, Guid workspaceId, Guid repositoryId)
    {
        var retVal = new FileAnalyze();
        if(repoItems.Count == 0) return retVal;
        var projects = ProjectManager.IdentifyProjects(repoItems);
        var components = new List<ThirdPartyComponent>();
        var relevantFiles = GetRelevantFiles(repoItems);
        var extractors = GetExtractors();
        foreach ( var extractor in extractors)
        {
            foreach (var file in relevantFiles)
            {
                if (extractor.CanHandle(file))
                {
                    var extractedComponents = extractor.ExtractComponents(file);
                    foreach (var extractedComponent in extractedComponents)
                    {
                        extractedComponent.UserId = file.UserId;
                        components.Add(extractedComponent);
                    }
                }
            }
        }
        foreach (var project in projects)
        {
            var normalizedProjectPath = Path.GetFullPath(project.Path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var projectComponents = components.Where(c => Path.GetFullPath(c.Path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).StartsWith(normalizedProjectPath, StringComparison.OrdinalIgnoreCase)).ToList();
            project.Components = projectComponents;
            project.WorkspaceId = workspaceId;
            project.RepositoryId = repositoryId;
        }
        retVal.Projects = projects;
        retVal.ThirdPartyComponents = components;
        return retVal;
    }
    public List<IComponentExtractor> GetExtractors()
    {
        var currentAssembly = Assembly.GetCallingAssembly();
        var types = currentAssembly.GetTypes().Where(t => t.IsClass && t.Name.EndsWith("Extractor") && !t.IsAbstract).ToList();
        var retVal = new List<IComponentExtractor>();
        if (types.Count == 0) return retVal;
        foreach (var extractorType in types)
        {
            var extractor = (IComponentExtractor)Activator.CreateInstance(extractorType)!;
            retVal.Add(extractor);
        }
        return retVal;
    }
}