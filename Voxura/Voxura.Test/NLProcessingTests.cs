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

        [Ignore("Consumes credits")]
        [Test]
        public async Task TestWithOpenAIApi()
        {
            await DoSimpleTest(true);
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
            
            //TODO test call arguments
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
            
            //mock system environment
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", "sk-myApiKey", EnvironmentVariableTarget.Process);

            var nlp = new NLProcessing(config);
            nlp.AIClient.OpenAIAuthentication.ApiKey.ShouldBe("sk-myApiKey");
        }

        [Test]
        public void APIKeyShouldOverrideEnvironmentAPIKey()
        {
            var config = new NLProcessingConfig()
            {
                OpenAIKeyLoadFromEnvironment = true,
                ApiKey = "sk-myApiKey"
            };
            
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", "sk-myApiKey2", EnvironmentVariableTarget.Process);
            var nlp = new NLProcessing(config);
            nlp.AIClient.OpenAIAuthentication.ApiKey.ShouldBe("sk-myApiKey");
        }

        private async Task DoSimpleTest(bool useRealApi)
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
            NLProcessing nlp;
            
            if (useRealApi)
            {
                nlp = new(config);
            }
            else
            {
                mockedHandler = Substitute.For<HttpMessageHandler>();
                nlp = new(config, new(OpenAIAuthentication.LoadFromEnv(), null, new HttpClient(mockedHandler)));
            }

            mockedHandler?.SetupResponse(HttpStatusCode.OK, @"{""StartDate"":""2023-03-14T00:00:00"",""EndDate"":""2023-03-14T00:00:00""}");
            
            var result = await nlp.ProcessAsync("Today is the PI day in 2023.");
            
            simplifyJson(result).ShouldBeOneOf(
                [
                    @"{""StartDate"":""2023-03-14"",""EndDate"":""2023-03-14""}",
                    @"{""StartDate"":""2023-03-14T00:00:00"",""EndDate"":""2023-03-14T23:59:59""}",
                    @"{""StartDate"":""2023-03-14T00:00:00"",""EndDate"":""2023-03-14T00:00:00""}"
                ]
            );

            mockedHandler?.SetupResponse(HttpStatusCode.OK, @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:29:59""}");

            result = await nlp.ProcessAsync("Let's meet tomorrow morning at 9:00 AM. I will have an appointment at 10:30 AM.");
            simplifyJson(result).ShouldBeOneOf(
                [
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:29:59""}",
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:30:00""}"
                ]
            );

            mockedHandler?.SetupResponse(HttpStatusCode.OK, @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:10:00""}");
            result = await nlp.ProcessAsync("I have to arrive to the other appointment at 10:30 AM, it takes approximately 20 mins at least to get there");
            
            simplifyJson(result).ShouldBeOneOf(
                [
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:10:00""}",
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:09:59""}"
                ]
            );
        }

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
                var task = await nlp.ProcessAsync("I would like to get a malformed response as a result");
                Assert.Fail("Exception expected");
            }
            catch (AggregateException e)
            {
                e.InnerException?.Message.ShouldContain("BadRequest");
            }
        }

        [Test]
        public void TestLowLevelAuthenticationError()
        {
            var config = new NLProcessingConfig
            {
                OpenAIKeyLoadFromEnvironment = true,
                ExtractionPrompt = "Need a simple answer in json format"
            };

            try
            {
                // pre validation test
                _ = new NLProcessing(config, new(new OpenAIAuthentication("invalid-api-key")));
                Assert.Fail("Exception expected");
            }
            catch (InvalidCredentialException e)
            {
                e.Message.ShouldContain("must start with 'sk-'");
            }


            var nlp = new NLProcessing(config, new(new OpenAIAuthentication("sk-invalid-api-key")));
            try
            {
                var task = nlp.ProcessAsync("3+3 is 6, am i right?");
                task.Wait();
                Assert.Fail("Exception expected");
            }
            catch (AggregateException e)
            {
                e.Message.ShouldContain("Unauthorized");
            }
        }

        [Test]
        public void TestContentFilteringError()
        {
            // TODO: handle and test it
            var config = new NLProcessingConfig
            {
                OpenAIKeyLoadFromEnvironment = true,
                ExtractionPrompt = "Content that triggered the filtering model as a json response",
                EnableDebug = true
            };

            var nlp = new NLProcessing(config);
            var task = nlp.ProcessAsync("I expect from you a valid content filter exception as a result");
            task.Wait();
            // TODO: Notify/Handle the exception if we have any
        }
    }
}
