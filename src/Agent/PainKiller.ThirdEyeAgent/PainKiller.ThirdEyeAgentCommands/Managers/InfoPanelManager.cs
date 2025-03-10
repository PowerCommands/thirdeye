using PainKiller.ThirdEyeAgentCommands.DomainObjects.InfoPanelContent;

namespace PainKiller.ThirdEyeAgentCommands.Managers
{
    public class InfoPanelManager(InfoPanelConfiguration configuration) : InfoPanelManagerBase(configuration, new HostInfoPanelContent(), new ThirdEyeInfoPanelContent(), new NvdInfoPanelContent());
}