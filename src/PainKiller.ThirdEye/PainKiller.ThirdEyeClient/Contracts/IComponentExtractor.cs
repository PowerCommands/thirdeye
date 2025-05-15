namespace PainKiller.ThirdEyeClient.Contracts;

public interface IComponentExtractor
{
    bool CanHandle(Item file);
    List<ThirdPartyComponent> ExtractComponents(Item file);
}