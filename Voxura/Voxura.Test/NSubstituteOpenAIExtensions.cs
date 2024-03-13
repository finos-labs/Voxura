using System.Net;
using System.Reflection;
using NSubstitute;
using NSubstitute.Core;

namespace Voxura.Test
{
    // Reference of the main idea from https://daninacan.com/how-to-mock-httpclient-in-c-using-nsubstitute-3-ways/
    public static class NSubstituteOpenAIExtensions
    {
        private static HttpMessageHandler SetupRequest(this HttpMessageHandler handler, HttpMethod method, string requestUri)
        {
            handler
                .GetType()
                .GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(handler, [
                    Arg.Is<HttpRequestMessage>(x =>
                        x.Method == method &&
                        x.RequestUri != null &&
                        x.RequestUri.ToString().EndsWith(requestUri)),
                    Arg.Any<CancellationToken>()
                ]
            );

            return handler;
        }

        private static ConfiguredCall ReturnsResponse(this HttpMessageHandler handler, HttpStatusCode statusCode, string responseContent)
        {
            return ((object)handler).Returns(
                Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = statusCode == HttpStatusCode.OK ? OpenAIResponse(responseContent) : OpenAIErrorResponse(responseContent)
                })
            );
        }

        private static StringContent OpenAIResponse(string responseContent)
        {
            // TODO: most of the parameters seemingly ignored at the moment in OpenAIClient, so let's KISS
            string modelName = "gpt-3.5-turbo-2021-07-01";

            String json = @"{
                ""id"": ""chatcmpl-" + Guid.NewGuid().ToString("n")[..29] + @""",
                ""object"": ""chat.completion"",
                ""created"": " + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + @",
                ""model"": """ + modelName + @""",
                ""choices"": [
                    {
                        ""index"": 0,
                        ""message"": {
                            ""role"": ""assistant"",
                            ""content"": " + responseContent + @"
                        },
                        ""logprobs"": null,
                        ""finish_reason"": ""stop""
                    }
                ],
                ""usage"": {
                ""prompt_tokens"": 230,
                ""completion_tokens"": 38,
                ""total_tokens"": 268
                },
                ""system_fingerprint"": ""fp_4f2ebda25a""
            }";

            return new StringContent(json);
        }

        private static StringContent OpenAIErrorResponse(string errorContent)
        {
            string json = @"""error"": {
                ""message"": """ + errorContent + @""",
                ""type"": ""invalid_request_error"",
                ""param"": ""messages"",
                ""code"": null
            }";

            Console.WriteLine("OpenAIErrorResponse: " + json);

            return new StringContent(json);
        }

        public static ConfiguredCall OpenAIChatResponse(this HttpMessageHandler handler, HttpStatusCode statusCode, string responseContent)
        {
            return handler.SetupRequest(HttpMethod.Post, "/v1/chat/completions").ReturnsResponse(statusCode, responseContent);
        }
    }
}