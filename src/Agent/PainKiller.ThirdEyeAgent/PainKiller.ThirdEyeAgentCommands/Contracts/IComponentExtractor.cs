using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Contracts;

public interface IComponentExtractor
{
    bool CanHandle(Item file);
    List<ThirdPartyComponent> ExtractComponents(Item file);
}