using PainKiller.ThirdEyeClient.DomainObjects;

namespace PainKiller.ThirdEyeClient.Contracts;

public interface IFileAnalyzeManager
{
    FileAnalyze AnalyzeRepo(List<Item> repoItems, Guid projectId, Guid repositoryId);
    List<IComponentExtractor> GetExtractors();
}