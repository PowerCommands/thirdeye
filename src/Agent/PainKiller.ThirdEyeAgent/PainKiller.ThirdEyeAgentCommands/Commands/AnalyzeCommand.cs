using PainKiller.ThirdEyeAgentCommands.BaseClasses;
using PainKiller.ThirdEyeAgentCommands.Managers.Workflows;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Check for vulnerabilities in your components",
                  disableProxyOutput: true,
                             example: "//Check for vulnerabilities in your components|analyze")]
    public class AnalyzeCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var workflow = new AnalyzeWorkspaceWorkflow(this, Configuration);
            workflow.Run();
            return Ok();
        }
        
    }
}