using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ThingsMobile.Tests
{
    public class ThingsMobileClientTests
    {
        [Fact]
        public async Task RequestIsSerializedCorrectly()
        {
            const string username = "username";
            const string token = "token";

            var handler = new DynamicHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);

                Assert.Null(req.Headers.Authorization);

                Assert.NotNull(req.Headers.UserAgent);
                var ua = Assert.Single(req.Headers.UserAgent);
                Assert.StartsWith("thingsmobile-dotnet/", ua.ToString());

                Assert.Equal("/services/business-api/simListLite", req.RequestUri?.AbsolutePath);
                Assert.Empty(req.RequestUri?.Query);
                Assert.Equal("https://api.thingsmobile.com/services/business-api/simListLite", req.RequestUri?.ToString());

                Assert.NotNull(req.Content);
                Assert.IsAssignableFrom<FormUrlEncodedContent>(req.Content);

                var expectedBody = $"username={username}&token={token}";
                var actualBody = await (req.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(""));
                Assert.Equal(expectedBody, actualBody);

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(await TestSamples.GetSimListResponseAsync(), Encoding.UTF8, "text/xml")
                };
            });
            var httpClient = new HttpClient(handler);

            var options = new ThingsMobileClientOptions
            {
                Username = username,
                Token = token,
            };
            var client = new ThingsMobileClient(options, httpClient);
            var response = await client.GetSimCardsLiteAsync();
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.IsSuccessful);
            Assert.Null(response.Error);
            Assert.NotNull(response.Resource);
            Assert.True(response.Resource!.IsSuccess);
            var sim = Assert.Single(response.Resource.Sims);
            Assert.Equal("447937557899", sim.Msisdn);
        }

        [Fact]
        public async Task ResponseIsDeserializedCorrectly_400()
        {
            const string username = "username";
            const string token = "token";

            var handler = new DynamicHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);

                Assert.Null(req.Headers.Authorization);

                Assert.NotNull(req.Headers.UserAgent);
                var ua = Assert.Single(req.Headers.UserAgent);
                Assert.StartsWith("thingsmobile-dotnet/", ua.ToString());

                Assert.Equal("/services/business-api/simListLite", req.RequestUri?.AbsolutePath);
                Assert.Empty(req.RequestUri?.Query);

                Assert.NotNull(req.Content);
                Assert.IsAssignableFrom<FormUrlEncodedContent>(req.Content);

                var expectedBody = $"username={username}&token={token}";
                var actualBody = await (req.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(""));
                Assert.Equal(expectedBody, actualBody);

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(await TestSamples.GetErrorAsync(), Encoding.UTF8, "text/xml")
                };
            });
            var httpClient = new HttpClient(handler);

            var options = new ThingsMobileClientOptions
            {
                Username = username,
                Token = token,
            };
            var client = new ThingsMobileClient(options, httpClient);
            var response = await client.GetSimCardsLiteAsync();
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(response.IsSuccessful);
            Assert.Null(response.Resource);
            Assert.NotNull(response.Error);
            Assert.False(response.Error!.IsSuccess);
            Assert.Equal("1", response.Error.Code);
            Assert.Equal("Error description", response.Error.Description);
        }
    }
}
