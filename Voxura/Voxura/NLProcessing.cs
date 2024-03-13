using OpenAI;
using OpenAI.Chat;

namespace Voxura.Core;

/// <summary>
/// NLProcessing is the heart of Voxura. It receives a natural language input and processes it to extract a JSON result.
/// </summary>
public class NLProcessing
{
    private readonly OpenAIClient _aiClient;

    private readonly NLProcessingConfig _config;

    private readonly List<Message> _chatHistory;

    public NLProcessing(NLProcessingConfig config, OpenAIClient? aiClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;

        _aiClient = aiClient ?? InitializeAIClient();
        _aiClient.EnableDebug = _config.EnableDebug;

        _chatHistory = [new(Role.System, _config.ExtractionPrompt)];
    }

    /// <summary>
    /// ProcessAsync receives a natural language input and processes it to extract a JSON result.
    /// Returns on the same thread as the caller.
    /// </summary>
    /// <returns>JSON result</returns>
    public Task<string> ProcessAsync(string input)
    {
        return ProcessAsyncInternal(input);
    }

    private async Task<string> ProcessAsyncInternal(string input)
    {
        // TODO: handle functions
        // TODO: better error handling on errors:
        //     _chatHistory.RemoveAt(_chatHistory.Count - 1);

        _chatHistory.Add(new(Role.User, input));
        var chatRequest = new ChatRequest(_chatHistory, _config.ModelName, responseFormat: ChatResponseFormat.Json);
        var response = await _aiClient.ChatEndpoint.GetCompletionAsync(chatRequest);

        var choice = response.FirstChoice;
        _chatHistory.Add(new(Role.Assistant, choice.Message.ToString()));
        return choice.Message.ToString();
    }

    private OpenAIClient InitializeAIClient()
    {
        if (_config.OpenAIKeyLoadFromEnvironment)
        {
            return new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
        }
        else if (_config.OpenAIConfigPath != null && File.Exists(_config.OpenAIConfigPath))
        {
            return new OpenAIClient(OpenAIAuthentication.LoadFromPath(_config.OpenAIConfigPath));
        }

        return new OpenAIClient(_config.ApiKey);
    }

}


public class NLProcessingConfig
{
    public string? ApiKey { get; set; }

    public bool OpenAIKeyLoadFromEnvironment { get; set; } = false;

    public string? OpenAIConfigPath { get; set; }

    public string ExtractionPrompt { get; set; } = "";

    public string ModelName { get; set; } = "gpt-3.5-turbo";

    public bool EnableDebug { get; set; } = false;
}
