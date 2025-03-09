using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContens;

public class HostInfoPanelContent : IInfoPanelContent
{
    public string GetText()
    {
        var configuration = PowerCommandServices.Service.ExtendedConfiguration;
        var text = $"{configuration.ThirdEyeAgent.Host} {configuration.ThirdEyeAgent.OrganizationName}    ";
        ShortText = text;
        return text;
    }
    public string? ShortText { get; private set; } = "";
}