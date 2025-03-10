namespace PainKiller.PowerCommands.Configuration.DomainObjects
{
    public static class ConfigurationGlobals
    {
        public const string ApplicationName = "ThirdEye";
        public const string MainConfigurationFile = "PowerCommandsConfiguration.yaml";
        public const string SecurityFileName = "security.yaml";
        public const string WhatsNewFileName = "whats_new.md";
        public const char ArraySplitter = '|';
        public const string SetupConfigurationFile = "setup.yaml";
        public const string EncryptionEnvironmentVariableName = "_encryptionManager";
        public const string UserNamePlaceholder = "%USERNAME%";
        public const string RoamingDirectoryPlaceholder = "$ROAMING$";
        public const string QueryPlaceholder = "$QUERY$";
        public const string DocsDirectoryName = "Docs";

        public const string NvdApiKeyName = "TE_api_key";
        public static string GetAccessTokenName(bool github) => github ? "TE_github_RepositoryToken" : "TE_RepositoryToken";
        public static readonly string ApplicationDataFolder = Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{nameof(PowerCommands)}", ApplicationName);
    }
}