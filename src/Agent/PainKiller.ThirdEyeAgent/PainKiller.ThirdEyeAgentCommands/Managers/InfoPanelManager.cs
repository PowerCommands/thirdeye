using PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContens;

namespace PainKiller.ThirdEyeAgentCommands.Managers
{
    public class InfoPanelManager(InfoPanelConfiguration configuration) : InfoPanelManagerBase(configuration, new HostInfoPanelContent(), new ThirdEyeInfoPanelContent(), new CurrentDirectoryInfoPanel());
}