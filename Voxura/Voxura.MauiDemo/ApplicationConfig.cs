namespace Voxura.MauiDemo;
public class ApplicationConfig
{
    /// <summary>
    ///  OpenAI API Key <see cref="NLProcessingConfig"/>
    /// </summary>
    public string? ApiKey { get; set; } = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "your-api-key-here";

    /// <summary>
    ///  OpenAI API Key can be loaded from the environment <see cref="NLProcessingConfig"/>
    /// </summary>
    public bool OpenAIKeyLoadFromEnvironment { get; set; } = false;

    /// <summary>
    /// Prompt to use for extraction <see cref="NLProcessingConfig"/>
    /// </summary>
    public string ExtractionPrompt { get; set; } = @"Below is a raw transcript of a user's verbal instructions to fill a form.
                                 Convert it to a JSON object that conforms to the TypeScript interface below.
                                 Ignore anything else. Answer only with the required object and nothing else !";

    /// <summary>
    /// Expected output result, it can be part of ExtractionPrompt <see cref="NLProcessingConfig"/>
    /// </summary>
    public string ExpectedOutput { get; set; } = @"
                                interface Contact {
                                    Id?: {
                                        Email?: string; // only valid email address or null
                                    };
                                    Name?: string;
                                }

                                interface Organization {
                                    Id?: {
                                        PERMID?: string;
                                    };
                                    Name?: string;
                                }

                                interface RFQ {
                                    Requestor?: Contact | Organization;
                                    Direction?: 'Buy' | 'Sell';
                                    Notional?: number as Int;
                                    StartDate?: date; // be strict with locale format or null
                                    EndDate?: date; // be strict with locale format or null
                                    RollConvention?: 'Following' | 'Modified Following' | 'Preceding';
                                    Trade?: {
                                        Product: string;  // The product or currency the user wants to buy or sell
                                    };
                                    Notes?: string;  // Any other information not captured by the above fields
                                }";
}
