using OpenAI;

namespace Voxura.Core;

/// <summary>
/// NLProcessing is the heart of Voxura. It receives a natural language input and processes it to extract a JSON result.
/// </summary>
public class NLProcessing
{
    private readonly NLProcessingConfig _config;

    public NLProcessing(NLProcessingConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
    }

    /// <summary>
    /// ProcessAsync receives a natural language input and processes it to extract a JSON result.
    /// Returns on the same thread as the caller.
    /// </summary>
    /// <returns>JSON result</returns>
    public Task<string> ProcessAsync(string input)
    {
        throw new NotImplementedException();
    }
}


public class NLProcessingConfig()
{
    public OpenAIClient AIClient { get; set; }  // TODO: make it a wrapper / interface later

    // TODO: add more configuration options

    public string ExtractionPrompt { get; set; }

    public string ModelName { get; set; }
}