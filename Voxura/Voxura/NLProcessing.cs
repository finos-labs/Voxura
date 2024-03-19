using System.Diagnostics;
using OpenAI;
using OpenAI.Chat;

namespace Voxura.Core;

/// <summary>
/// NLProcessing is the heart of Voxura. It receives a natural language input and processes it to extract a JSON result.
/// This class represents a single session.
/// </summary>
public class NLProcessing
{
    public OpenAIClient AIClient { get; private set; }
    private readonly NLProcessingConfig _config;

    public NLProcessing(NLProcessingConfig config, OpenAIClient? aiClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;

        AIClient = aiClient ?? InitializeAIClient();
        AIClient.EnableDebug = _config.EnableDebug;
    }

    /// <summary>
    /// ProcessAsync receives a natural language input and processes it to extract a JSON result.
    /// Returns on the same thread as the caller.
    /// </summary>
    /// <returns>JSON result</returns>
    public async Task<string> ProcessAsync(string input) {
        return await ProcessAsync([ input ]);
    }

    /// <summary>
    /// ProcessAsync receives a natural language input and it's history and processes it to extract a JSON result.
    /// Returns on the same thread as the caller.
    /// </summary>
    /// <returns>JSON result</returns>
    public async Task<string> ProcessAsync(List<string> input)
    {
        // TODO: better error handling on errors:

        List<Message> chatHistory = new(); // this is not a real chat, we need a new history for every request

        chatHistory.Add(new Message(Role.System, _config.ExtractionPrompt));
        foreach (var text in input)
        {
            chatHistory.Add(new Message(Role.User, text));
        }

        var chatRequest = new ChatRequest(chatHistory, _config.ModelName, responseFormat: ChatResponseFormat.Json);
        var response = await AIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

        var choice = response.FirstChoice;

        // TODO: error handling

        return choice.Message.ToString();
    }

    private OpenAIClient InitializeAIClient()
    {
        if (_config.ApiKey != null)
        {
            return new OpenAIClient(_config.ApiKey);
        }

        if (_config.OpenAIKeyLoadFromEnvironment)
        {
            return new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
        }

        throw new Exception("Either provide an ApiKey or set OpenAIKeyLoadFromEnvironment");
    }
}

//TODO: Make it work with Azure OpenAI Service as well
/// <summary>
/// Config object for <see cref="NLProcessing"/>
/// </summary>
public class NLProcessingConfig
{
    /// <summary>
    /// API key
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Whether to load the API key from the environment variable "OPENAI_API_KEY", if <see cref="ApiKey"/> is <code>null</code>
    /// </summary>
    public bool OpenAIKeyLoadFromEnvironment { get; set; } = true;

    /// <summary>
    /// Prompt to use for extraction
    /// </summary>
    public string ExtractionPrompt { get; set; } = "";

    /// <summary>
    /// Model name to use
    /// </summary>
    public string ModelName { get; set; } = "gpt-3.5-turbo";

    /// <summary>
    /// Whether to enable debug mode
    /// </summary>
    public bool EnableDebug { get; set; } = false;
}
