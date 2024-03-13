using Voxura.Core;
using Shouldly;
using System.Text.RegularExpressions;
using OpenAI;
using System.Net;
using NSubstitute;
using System.Security.Authentication;

namespace Voxura.Test
{
    public class TestSimpleCase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestWithOpenAIApi()
        {
            DoSimpleTest(true);
        }

        [Test]
        public void TestWithOpenAIMock()
        {
            DoSimpleTest(false);
        }

        private static void DoSimpleTest(bool useRealApi)
        {
            var config = new NLProcessingConfig
            {
                OpenAIKeyLoadFromEnvironment = true,
                ExtractionPrompt = @"Below is a raw transcript of a user's verbal instructions to fill a form.
                                     Convert it to a JSON object that conforms the RFQ TypeScript interface below.
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

            mockedHandler?.OpenAIChatResponse(HttpStatusCode.OK, @"{""StartDate"":""2023-03-14T00:00:00"",""EndDate"":""2023-03-14T00:00:00""}");
            var result = nlp.ProcessAsync("Today is the PI day in 2023.").ConfigureAwait(false).GetAwaiter().GetResult();
            simplifyJson(result).ShouldBeOneOf(
                [
                    @"{""StartDate"":""2023-03-14"",""EndDate"":""2023-03-14""}",
                    @"{""StartDate"":""2023-03-14T00:00:00"",""EndDate"":""2023-03-14T23:59:59""}",
                    @"{""StartDate"":""2023-03-14T00:00:00"",""EndDate"":""2023-03-14T00:00:00""}"
                ]
            );

            mockedHandler?.OpenAIChatResponse(HttpStatusCode.OK, @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:29:59""}");
            result = nlp.ProcessAsync("Let's meet tomorrow morning at 9:00 AM. I will have an appointment at 10:30 AM.").ConfigureAwait(false).GetAwaiter().GetResult();
            simplifyJson(result).ShouldBeOneOf(
                [
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:29:59""}",
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:30:00""}"
                ]
            );

            mockedHandler?.OpenAIChatResponse(HttpStatusCode.OK, @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:10:00""}");
            result = nlp.ProcessAsync("I have to arrive to the other appointment at 10:30 AM, it takes approximately 20 mins at least to get there").ConfigureAwait(false).GetAwaiter().GetResult();
            simplifyJson(result).ShouldBeOneOf(
                [
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:10:00""}",
                    @"{""StartDate"":""2023-03-15T09:00:00"",""EndDate"":""2023-03-15T10:09:59""}"
                ]
            );
        }

        [Test]
        public void TestMalformedPrompt()
        {
            var config = new NLProcessingConfig
            {
                OpenAIKeyLoadFromEnvironment = true,
                ExtractionPrompt = "Test for malformed response which must return an ASCII text"
            };

            var nlp = new NLProcessing(config);
            try
            {
                var task = nlp.ProcessAsync("I would like to get a malformed response as a result");
                task.Wait();
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
