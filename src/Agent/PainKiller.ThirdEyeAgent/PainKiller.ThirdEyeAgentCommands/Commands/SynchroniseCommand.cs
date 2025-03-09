using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Synchronise with your source code server",
                             options: "init",
                  disableProxyOutput: true,
                             example: "Synchronise with your source code server//|synchronise")]
    public class SynchroniseCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var synchronisationManager = new SynchronisationManager(GitManager, ObjectStorage, AnalyzeManager, this, configuration.ThirdEyeAgent);
            if(HasOption("init")) synchronisationManager.InitializeOrganization();
            else synchronisationManager.UpdateOrganization();
            ObjectStorage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
            return Ok();
        }
    }
}