namespace Voxura.MauiDemo;
public class ApplicationConfig
{
    /// <summary>
    ///  OpenAI API Key <see cref="NLProcessingConfig"/>
    /// </summary>
    public string? ApiKey { get; set; } = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "your-api-key-here";

    /// <summary>
    /// Model name to use <see cref="NLProcessingConfig"/
    /// </summary>
    public string? ModelName { get; set; } = "chat-gpt-3.5-turbo";

    /// <summary>
    ///  OpenAI API Key can be loaded from the environment <see cref="NLProcessingConfig"/>
    /// </summary>
    public bool OpenAIKeyLoadFromEnvironment { get; set; } = false;
}
