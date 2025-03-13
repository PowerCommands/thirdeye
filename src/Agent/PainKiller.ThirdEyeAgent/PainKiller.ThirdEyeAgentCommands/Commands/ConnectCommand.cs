using PainKiller.ThirdEyeAgentCommands.BaseClasses;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Text connection to server",
                  disableProxyOutput: true,
                             example: "//Test connection|conenct")]
    public class ConnectCommand(string identifier, PowerCommandsConfiguration configuration) :ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            GitManager.Connect();
            return Ok();
        }
    }
}