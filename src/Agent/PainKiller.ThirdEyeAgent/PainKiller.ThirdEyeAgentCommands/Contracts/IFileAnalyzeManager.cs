﻿using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IFileAnalyzeManager
{
    FileAnalyze AnalyzeRepo(List<Item> repoItems, Guid projectId, Guid repositoryId);
    List<IComponentExtractor> GetExtractors();
}