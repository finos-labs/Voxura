using Voxura.Core;
using Shouldly;
using System.Text.RegularExpressions;
using OpenAI;
using System.Net;
using NSubstitute;
using System.Security.Authentication;

namespace Voxura.Test
{
    public class NLProcessingTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Explicit("Consumes credits")]
        [Test]
        public async Task TestWithOpenAIApi()
        {
            var config = new NLProcessingConfig
            {
                OpenAIKeyLoadFromEnvironment = true,
                ExtractionPrompt = @"Below is a raw transcript of a user's verbal instructions to fill a form.
                                     Convert it to a JSON object that conforms to the TypeScript interface below.
                                     Ignore anything else. Answer only with the required object and nothing else !

                                     interface RFQ {
                                        StartDate: datetime;
                                        EndDate: datetime;
                                     }
                                     ",
                EnableDebug = false
            };

            Func<string, string> simplifyJson = json => Regex.Replace(json, "\\s+", "");

            HttpMessageHandler? mockedHandler = null;
            NLProcessing nlp = new(config);

            var result = await nlp.ProcessAsync("Today is the PI day in 2023.");
            simplifyJson(result).ShouldBeOneOf(
                [
                    """{"StartDate":"2023-03-14","EndDate":"2023-03-14"}""",
                    """{"StartDate":"2023-03-14T00:00:00","EndDate":"2023-03-14T23:59:59"}""",
                    """{"StartDate":"2023-03-14T00:00:00","EndDate":"2023-03-14T00:00:00"}"""
                ]
            );
        }

        [Test]
        public async Task TestWithOpenAIMock()
        {
            var config = new NLProcessingConfig()
            {
                ApiKey = "sk-myApiKey",
                ModelName = "myModelName",
                ExtractionPrompt = "my prompt",
                EnableDebug = true,
            };

            var mockedHandler = Substitute.For<HttpMessageHandler>();
            var nlp = new NLProcessing(config, new OpenAIClient(null, null, new HttpClient(mockedHandler)));

            const string simpleJson = """
                                      {
                                          "foo": "bar"
                                      }
                                      """;

            mockedHandler.SetupResponse(HttpStatusCode.OK, simpleJson);
            mockedHandler.ClearReceivedCalls();

            var result = await nlp.ProcessAsync("foobar?");
            result.ShouldBe(simpleJson);

            mockedHandler.ReceivedCalls().Count().ShouldBe(1);
            var call = mockedHandler.ReceivedCalls().First();
            
            call.GetMethodInfo().Name.ShouldBe("SendAsync");
            var httpReqMsg = call.GetOriginalArguments()[0] as HttpRequestMessage;
            httpReqMsg.ShouldNotBeNull();
            

            //TODO test call arguments (Do we need low level test?)
            // - method
            // - requestUri
            // - content
            // - headers
            // - system prompt, prompt
            // - JSON response
        }

        [Test]
        public void TestConfig()
        {
            var config = new NLProcessingConfig()
            {
                ApiKey = "sk-myApiKey",
                ModelName = "myModelName",
                ExtractionPrompt = "my prompt",
                EnableDebug = true,
                OpenAIKeyLoadFromEnvironment = false
            };

            var nlp = new NLProcessing(config);
            nlp.AIClient.EnableDebug.ShouldBeTrue();
            nlp.AIClient.OpenAIAuthentication.ApiKey.ShouldBe("sk-myApiKey");
        }

        [Test]
        public void ConfigShouldThrowIfNoApiKey()
        {
            var config = new NLProcessingConfig()
            {
                OpenAIKeyLoadFromEnvironment = false
            };

            ShouldThrowExtensions.ShouldThrow(() => new NLProcessing(config), typeof(Exception));
        }

        [Test]
        public void TestEnvironmentApiKey()
        {
            var config = new NLProcessingConfig()
            {
                OpenAIKeyLoadFromEnvironment = true
            };

            VerifyEnvironmentVariableConfig(config, "sk-myEnvApiKey", "sk-myEnvApiKey");
        }

        [Test]
        public void APIKeyShouldOverrideEnvironmentAPIKey()
        {
            var config = new NLProcessingConfig()
            {
                OpenAIKeyLoadFromEnvironment = true,
                ApiKey = "sk-myApiKey"
            };

            VerifyEnvironmentVariableConfig(config, "sk-myEnvApiKey", "sk-myApiKey");
        }
        private void VerifyEnvironmentVariableConfig(NLProcessingConfig config, string envApiKey, string expectedApiKey)
        {
            // the environment variable should not be affected for the rest of the tests
            string? originalEnvKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            try
            {
                Environment.SetEnvironmentVariable("OPENAI_API_KEY", envApiKey, EnvironmentVariableTarget.Process);
                var nlp = new NLProcessing(config);
                nlp.AIClient.OpenAIAuthentication.ApiKey.ShouldBe(expectedApiKey);
            }
            finally
            {
                // finally restore the original environment variable
                if (originalEnvKey != null)
                {
                    Environment.SetEnvironmentVariable("OPENAI_API_KEY", originalEnvKey, EnvironmentVariableTarget.Process);
                }
            }
        }

        private async Task DoSimpleTest(bool useRealApi)
        {

        }

        [Explicit("Consumes credits")]
        [Test]
        public async Task TestMalformedPrompt()
        {
            var config = new NLProcessingConfig
            {
                OpenAIKeyLoadFromEnvironment = true,
                ExtractionPrompt = "Test for malformed response which must return an ASCII text"
            };

            var nlp = new NLProcessing(config);
            try
            {
                var result = await nlp.ProcessAsync("I would like to get a malformed response as a result");
                Assert.Fail("Exception expected");
            }
            catch (HttpRequestException e)
            {
                e.InnerException?.Message.ShouldContain("BadRequest");
            }
        }

        [Test]
        public void TestLowLevelAuthenticationError()
        {
            var config = new NLProcessingConfig
            {
                ExtractionPrompt = "Need a simple answer in json format",
                ApiKey = "invalid-api-key"
            };

            try
            {
                // pre validation test
                _ = new NLProcessing(config);
                Assert.Fail("Exception expected");
            }
            catch (InvalidCredentialException e)
            {
                e.Message.ShouldContain("must start with 'sk-'");
            }
        }

        [Test]
        public async Task TestAuthenticationError()
        {
            var config = new NLProcessingConfig
            {
                ExtractionPrompt = "Need a simple answer in json format",
                ApiKey = "sk-invalid-api-key"
            };

            var mockedHandler = Substitute.For<HttpMessageHandler>();
            var nlp = new NLProcessing(config, new(new OpenAIAuthentication("sk-invalid-api-key"), null, new HttpClient(mockedHandler)));
            try
            {
                mockedHandler?.SetupResponse(HttpStatusCode.Unauthorized, "Invalid API key");
                var result = await nlp.ProcessAsync("3+3 is 6, am i right?");
                Assert.Fail("Exception expected");
            }
            catch (HttpRequestException e)
            {
                e.Message.ShouldContain("Unauthorized");
            }
        }

        [Ignore("Not implemented yet")]
        [Test]
        public async Task TestContentFilteringError()
        {
            // TODO: handle and test it
            var config = new NLProcessingConfig
            {
                OpenAIKeyLoadFromEnvironment = true,
                ExtractionPrompt = "Content that triggered the filtering model as a json response",
                EnableDebug = true
            };

            var mockedHandler = Substitute.For<HttpMessageHandler>();
            var nlp = new NLProcessing(config, new(OpenAIAuthentication.LoadFromEnv(), null, new HttpClient(mockedHandler)));

            var result = await nlp.ProcessAsync("I expect from you a valid content filter exception as a result");
            // TODO: Notify/Handle the exception if we have any
        }
    }
}
