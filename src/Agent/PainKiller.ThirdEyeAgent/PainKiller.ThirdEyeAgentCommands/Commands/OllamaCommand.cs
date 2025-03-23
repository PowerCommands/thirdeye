using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using PainKiller.PowerCommands.Core.Commands;

namespace PainKiller.ThirdEyeAgentCommands.Commands;

[PowerCommandDesign( description: "Manage your CVEs from National Vulnerability Database (NVD), request an API key from https://nvd.nist.gov/developers/request-an-api-key.",
    disableProxyOutput: true,
    options: "!api-key|fetch|push-update-file",
    example: "//Update your cve:s from the last page you collected.|nvd")]
public class OllamaCommand(string identifier, PowerCommandsConfiguration config) : MasterCommando(identifier, config)
{
    private const int OllamaPort = 11434;
    private static readonly string OllamaBaseAddress = $"http://localhost:{OllamaPort}";

    private readonly List<ChatMessage> _messages = new();

    public override RunResult Run()
    {
        if (!IsOllamaServerRunning())
        {
            StartOllamaServer();
            Thread.Sleep(2000);

            if (!IsOllamaServerRunning())
            {
                WriteLine("Kan inte starta Ollama-servern.");
                return BadParameterError("Kan inte starta Ollama-servern.");
            }
        }

        WriteLine("Startar konversation med modellen (skriv /bye för att avsluta):");

        while (true)
        {
            Write("> ");
            var userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput)) continue;
            if (userInput.Trim().Equals("/bye", StringComparison.OrdinalIgnoreCase)) break;

            _messages.Add(new ChatMessage("user", userInput));
            var response = SendChatToOllama("gemma3:latest").GetAwaiter().GetResult();

            WriteLine(response);
            _messages.Add(new ChatMessage("assistant", response));
        }

        return Ok();
    }

    private bool IsOllamaServerRunning()
    {
        try
        {
            using var client = new TcpClient("localhost", OllamaPort);
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private void StartOllamaServer()
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

    private async Task<string> SendChatToOllama(string model)
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri(OllamaBaseAddress) };

        var payload = new
        {
            model,
            messages = _messages,
            stream = false
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var response = await httpClient.PostAsync("/api/chat",
            new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            return $"Fel från Ollama-servern: {response.StatusCode}";
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(jsonResponse);
        if (document.RootElement.TryGetProperty("message", out var messageObj) &&
            messageObj.TryGetProperty("content", out var content))
        {
            return content.GetString() ?? "<Tomt svar>";
        }

        return "Oväntat svar från Ollama-servern.";
    }

    private record ChatMessage(string role, string content);
}