using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Synchronise with your source code server",
                             options: "init",
                  disableProxyOutput: true,
                             example: "//Synchronise with your source code server|synchronise|//Initialize your organisation, will reset your current stored organisation and components.|synchronise --init")]
    public class SynchroniseCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var synchronisationManager = new SynchronisationManager(GitManager, Storage, AnalyzeManager, this, Configuration.ThirdEyeAgent);
            if(HasOption("init")) synchronisationManager.InitializeOrganization();
            else synchronisationManager.UpdateOrganization();
            Storage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
            return Ok();
        }
    }
}