namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Text connection to server",
                  disableProxyOutput: true,
                             example: "//Text connection|conenct")]
    public class ConnectCommand(string identifier, PowerCommandsConfiguration configuration) :ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            GitManager.Connect();
            return Ok();
        }
    }
}