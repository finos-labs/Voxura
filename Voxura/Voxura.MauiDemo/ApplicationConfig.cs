using System;

namespace Voxura.MauiDemo
{
    public class ApplicationConfig
    {
        /// <summary>
        ///  OpenAI API Key <see cref="NLProcessingConfig"/>
        /// </summary>
        public string? ApiKey { get; private set; } = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        /// <summary>
        /// Model name to use <see cref="NLProcessingConfig"/
        /// </summary>
        public string? ModelName { get; set; } = "gpt-3.5-turbo";

        /// <summary>
        ///  OpenAI API Key can be loaded from the environment <see cref="NLProcessingConfig"/>
        /// </summary>
        public bool OpenAIKeyLoadFromEnvironment { get; set; } = false;

        public bool Verify() {
            string storedApiKey = Preferences.Get("OpenAI.ApiKey", "");
            if (!string.IsNullOrWhiteSpace(storedApiKey) && storedApiKey != ApiKey)
            {
                ApiKey = storedApiKey;
            }

            return OpenAIKeyLoadFromEnvironment || !string.IsNullOrWhiteSpace(ApiKey);
        }

        private bool LoadFromPreference()
        {
            return !string.IsNullOrWhiteSpace(ApiKey);
        }
    }

}