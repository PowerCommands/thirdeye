﻿using PainKiller.ThirdEyeAgentCommands.Managers;

namespace PainKiller.ThirdEyeAgentCommands.Commands
{
    [PowerCommandDesign( description: "Synchronise with your git server",
                             options: "init",
                  disableProxyOutput: true,
                             example: "//Synchronise with your git server|git|//Initialize your organisation, will reset your current stored organisation and components.|git --init")]
    public class GitCommand(string identifier, PowerCommandsConfiguration configuration) : ThirdEyeBaseCommando(identifier, configuration)
    {
        public override RunResult Run()
        {
            var workspaceManager = new WorkspaceManager(GitManager, Storage, AnalyzeManager, this, Configuration.ThirdEyeAgent);
            if(HasOption("init")) workspaceManager.InitializeOrganization();
            else workspaceManager.UpdateOrganization();
            Storage.ReLoad();
            IPowerCommandServices.DefaultInstance?.InfoPanelManager.Display();
            return Ok();
        }
    }
}