namespace PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContent;

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