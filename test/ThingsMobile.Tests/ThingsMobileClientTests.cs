using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThingsMobile;
using ThingsMobile.Tests;
using Xunit;

namespace productboard.Tests
{
    public class ThingsMobileClientTests
    {
        private const string OkResponse = "<result>" +
                                                    "<done>true</done>" +
                                                    "<sims>" +
                                                        "<sim>" +
                                                            "<activationDate>2018-02-07 15:52:49</activationDate>" +
                                                            "<balance>30</balance>" +
                                                            "<blockSimAfterExpirationDate>1</blockSimAfterExpirationDate>" +
                                                            "<blockSimDaily>1</blockSimDaily>" +
                                                            "<blockSimMonthly>1</blockSimMonthly>" +
                                                            "<blockSimTotal>1</blockSimTotal>" +
                                                            "<dailyTraffic>123</dailyTraffic>" +
                                                            "<dailyTrafficThreshold>1000000</dailyTrafficThreshold>" +
                                                            "<expirationDate>2018-02-20 00:00:00</expirationDate>" +
                                                            "<iccid>8944501312167518236</iccid>" +
                                                            "<lastConnectionDate></lastConnectionDate>" +
                                                            "<monthlyTraffic>1234</monthlyTraffic>" +
                                                            "<monthlyTrafficThreshold>2000000</monthlyTrafficThreshold>" +
                                                            "<msisdn>447937557899</msisdn>" +
                                                            "<name>name</name>" +
                                                            "<plan>default</plan>" +
                                                            "<status>active</status>" +
                                                            "<tag>tag</tag>" +
                                                            "<totalTraffic>0</totalTraffic>" +
                                                            "<totalTrafficThreshold>3000000</totalTrafficThreshold>" +
                                                            "<type>AllInOne Sim</type>" +
                                                            "<cdrs>" +
                                                            "<cdr>" +
                                                            "<cdrImsi>123456</cdrImsi>" +
                                                            "<cdrDateStart>2017-01-20 12:45:00</cdrDateStart>" +
                                                            "<cdrDateStop>2017-01-20 12:50:00</cdrDateStop>" +
                                                            "<cdrNetwork>Zone 1</cdrNetwork>" +
                                                            "<cdrCountry>Zone 1</cdrCountry>" +
                                                            "<cdrTraffic>123456</cdrTraffic>" +
                                                            "<cdrOperator> ITAWI</cdrOperator>" +
                                                            "</cdr>" +
                                                            "</cdrs>" +
                                                        "</sim>" +
                                                    "</sims>" +
                                                "</result>";

        private const string ErrorResponse = "<result>" +
                                                "<done>false</done>" +
                                                "<errorCode>1</errorCode>" +
                                                "<errorMessage>Error description</errorMessage>" +
                                            "</result>";

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

                Assert.Equal("/services/business-api/simList", req.RequestUri.AbsolutePath);
                Assert.Empty(req.RequestUri.Query);

                Assert.NotNull(req.Content);
                Assert.IsAssignableFrom<FormUrlEncodedContent>(req.Content);

                var expectedBody = $"username={username}&token={token}";
                var actualBody = await req.Content.ReadAsStringAsync();
                Assert.Equal(expectedBody, actualBody);
                
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(OkResponse, Encoding.UTF8, "text/xml")
                };
            });
            var httpClient = new HttpClient(handler);

            var options = new ThingsMobileClientOptions
            {
                Username = username,
                Token = token,
            };
            var client = new ThingsMobileClient(options, httpClient);
            var response = await client.ListSimCardsAsync();
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.IsSuccessful);
            Assert.Null(response.Error);
            Assert.NotNull(response.Resource);
            Assert.True(response.Resource.IsSuccess);
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

                Assert.Equal("/services/business-api/simList", req.RequestUri.AbsolutePath);
                Assert.Empty(req.RequestUri.Query);

                Assert.NotNull(req.Content);
                Assert.IsAssignableFrom<FormUrlEncodedContent>(req.Content);

                var expectedBody = $"username={username}&token={token}";
                var actualBody = await req.Content.ReadAsStringAsync();
                Assert.Equal(expectedBody, actualBody);

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(ErrorResponse, Encoding.UTF8, "text/xml")
                };
            });
            var httpClient = new HttpClient(handler);

            var options = new ThingsMobileClientOptions
            {
                Username = username,
                Token = token,
            };
            var client = new ThingsMobileClient(options, httpClient);
            var response = await client.ListSimCardsAsync();
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(response.IsSuccessful);
            Assert.Null(response.Resource);
            Assert.NotNull(response.Error);
            Assert.False(response.Error.IsSuccess);
            Assert.Equal("1", response.Error.Code);
            Assert.Equal("Error description", response.Error.Description);
        }
    }
}
