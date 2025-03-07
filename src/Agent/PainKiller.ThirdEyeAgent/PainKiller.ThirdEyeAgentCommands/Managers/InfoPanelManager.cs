namespace PainKiller.ThirdEyeAgentCommands.Managers
{
    public class InfoPanelManager(InfoPanelConfiguration configuration) : InfoPanelManagerBase(configuration, new UserNameInfoPanelContent(), new TimeInfoPanelContent(), new CurrentDirectoryInfoPanel());
}