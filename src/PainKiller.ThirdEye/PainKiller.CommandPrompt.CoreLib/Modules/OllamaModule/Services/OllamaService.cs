using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using PainKiller.CommandPrompt.CoreLib.Modules.OllamaModule.DomainObjects;
using Microsoft.Extensions.Logging;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using PainKiller.CommandPrompt.CoreLib.Modules.OllamaModule.Contracts;

namespace PainKiller.CommandPrompt.CoreLib.Modules.OllamaModule.Services;
public class OllamaService : IOllamaService
{
    private readonly ILogger<OllamaService> _logger = LoggerProvider.CreateLogger<OllamaService>();
    private readonly List<ChatMessage> _messages = [];

    private static IOllamaService? _instance;
    private static readonly Lock LockObject = new();
    private readonly string _baseAddress;
    private readonly int _port;
    private readonly string _model;

    private OllamaService(string baseAddress, int port, string model)
    {
        _baseAddress = baseAddress;
        _port = port;
        _model = model;
    }

    public static IOllamaService GetInstance(string baseAddress, int port, string model)
    {
        if (_instance != null) return _instance;
        lock (LockObject)
        {
            _instance ??= new OllamaService(baseAddress, port, model);
        }
        return _instance;
    }

    public bool IsOllamaServerRunning()
    {
        try
        {
            using var client = new TcpClient(_baseAddress, _port);
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }
    public void StartOllamaServer()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = "serve",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process.Start(startInfo);
    }
    public async Task<string> SendChatToOllama()
    {
        var ollamaBaseAddress = $"http://{_baseAddress}:{_port}";
        using var httpClient = new HttpClient { BaseAddress = new Uri(ollamaBaseAddress) };

        var payload = new
        {
            model = _model,
            messages = _messages,
            stream = false
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var response = await httpClient.PostAsync("/api/chat",
            new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error from ollama server: {response.StatusCode}");
            return $"Error from ollama server: {response.StatusCode}";
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(jsonResponse);
        if (document.RootElement.TryGetProperty("message", out var messageObj) &&
            messageObj.TryGetProperty("content", out var content))
        {
            return content.GetString() ?? "<Empty answer>";
        }

        return "Unexpected respond from Ollama-server.";
    }
    public void AddMessage(ChatMessage message) => _messages.Add(message);
}