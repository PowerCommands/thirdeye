namespace PainKiller.ThirdEyeAgentCommands.Configuration
{
    public class PowerCommandsConfiguration : CommandsConfiguration
    {
        public ToolbarConfiguration? StartupToolbar { get; set; }
        public EncryptionConfiguration Encryption { get; set; } = new();
        public ThirdEyeConfiguration ThirdEyeAgent { get; set; } = new();
    }
}