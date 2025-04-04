using PainKiller.CommandPrompt.CoreLib.Core.Contracts;
using PainKiller.CommandPrompt.CoreLib.Core.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.ThirdEyeClient.BaseClasses;

namespace PainKiller.ThirdEyeClient.Commands;

[CommandDesign( description: "Text connection to server",
    examples: ["//Test connection|conenct"])]
public class ConnectCommand(string identifier) :ThirdEyeBaseCommando(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        GitManager.Connect();
        return Ok();
    }
}