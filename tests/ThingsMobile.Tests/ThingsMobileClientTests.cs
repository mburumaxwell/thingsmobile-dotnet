using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using Xunit;

namespace ThingsMobile.Tests;

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
            Assert.NotNull(req.RequestUri?.Query);
            Assert.Empty(req.RequestUri.Query);
            Assert.Equal("https://api.thingsmobile.com/services/business-api/simListLite", req.RequestUri?.ToString());

            Assert.NotNull(req.Content);
            Assert.IsType<FormUrlEncodedContent>(req.Content, exactMatch: false);

            var expectedBody = $"username={username}&token={token}";
            var actualBody = await (req.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(""));
            Assert.Equal(expectedBody, actualBody);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(await TestSamples.GetSimListResponseAsync(), Encoding.UTF8, "text/xml")
            };
        });

        var services = new ServiceCollection()
            .AddThingsMobile(options =>
            {
                options.Username = username;
                options.Token = token;
            })
            .ConfigurePrimaryHttpMessageHandler(() => handler)
            .Services.BuildServiceProvider();
        var client = services.GetRequiredService<ThingsMobileClient>();

        var response = await client.GetSimCardsLiteAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessful);
        Assert.Null(response.Error);
        Assert.NotNull(response.Resource);
        Assert.True(response.Resource!.IsSuccess);
        Assert.NotNull(response.Resource.Sims);
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
            Assert.NotNull(req.RequestUri?.Query);
            Assert.Empty(req.RequestUri.Query);

            Assert.NotNull(req.Content);
            Assert.IsType<FormUrlEncodedContent>(req.Content, exactMatch: false);

            var expectedBody = $"username={username}&token={token}";
            var actualBody = await (req.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(""));
            Assert.Equal(expectedBody, actualBody);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(await TestSamples.GetErrorAsync(), Encoding.UTF8, "text/xml")
            };
        });

        var services = new ServiceCollection()
            .AddThingsMobile(options =>
            {
                options.Username = username;
                options.Token = token;
            })
            .ConfigurePrimaryHttpMessageHandler(() => handler)
            .Services.BuildServiceProvider();
        var client = services.GetRequiredService<ThingsMobileClient>();

        var response = await client.GetSimCardsLiteAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.IsSuccessful);
        Assert.Null(response.Resource);
        Assert.NotNull(response.Error);
        Assert.False(response.Error!.IsSuccess);
        Assert.Equal("1", response.Error.Code);
        Assert.Equal("Error description", response.Error.Description);
    }

    [Fact]
    public async Task ResponseIsDeserializedCorrectly_CdrPaginated()
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

            Assert.Equal("/services/business-api/getCdrPaginated", req.RequestUri?.AbsolutePath);
            Assert.NotNull(req.RequestUri?.Query);
            Assert.Empty(req.RequestUri.Query);

            Assert.NotNull(req.Content);
            Assert.IsType<FormUrlEncodedContent>(req.Content, exactMatch: false);

            var expectedBody = string.Join("&", [
                "msisdnList=1234567890",
                "startDateRange=0001-01-01+00%3A00%3A00",
                "endDateRange=9999-12-31+23%3A59%3A59",
                $"username={username}",
                $"token={token}"
            ]);
            var actualBody = await (req.Content?.ReadAsStringAsync(ct) ?? Task.FromResult(""));
            Assert.Equal(expectedBody, actualBody);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(await TestSamples.GetCdrPaginatedResponseAsync(), Encoding.UTF8, "text/xml")
            };
        });

        var services = new ServiceCollection()
            .AddThingsMobile(options =>
            {
                options.Username = username;
                options.Token = token;
            })
            .ConfigurePrimaryHttpMessageHandler(() => handler)
            .Services.BuildServiceProvider();
        var client = services.GetRequiredService<ThingsMobileClient>();

        var response = await client.GetCdrAsync(["1234567890"],
                                                DateTimeOffset.MinValue,
                                                DateTimeOffset.MaxValue,
                                                cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessful);
        Assert.NotNull(response.Resource);
        Assert.Null(response.Error);
        Assert.NotNull(response.Resource?.CallDetailRecords);
        var cdr = Assert.Single(response.Resource.CallDetailRecords);
        Assert.Equal("Zone 1", cdr.Network);
        Assert.Equal("882360001975037", cdr.Msisdn);
    }
}
