using PainKiller.CommandPrompt.CoreLib.Modules.OllamaModule.DomainObjects;

namespace PainKiller.CommandPrompt.CoreLib.Modules.OllamaModule.Contracts;

public interface IOllamaService
{
    bool IsOllamaServerRunning();
    void StartOllamaServer();
    Task<string> SendChatToOllama();
    void AddMessage(ChatMessage message);
}