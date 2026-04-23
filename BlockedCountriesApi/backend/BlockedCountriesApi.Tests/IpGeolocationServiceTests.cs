using System.Net;
using System.Text;
using System.Text.Json;
using BlockedCountriesApi.Models;
using BlockedCountriesApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlockedCountriesApi.Tests;

/// <summary>
/// A simple HttpMessageHandler stub that always returns a preset response.
/// </summary>
internal class StubHttpHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public StubHttpHandler(HttpResponseMessage response) => _response = response;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(_response);
}

public class IpGeolocationServiceTests
{
    private IpGeolocationService BuildService(HttpResponseMessage httpResponse)
    {
        var client = new HttpClient(new StubHttpHandler(httpResponse));
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient("IpApi")).Returns(client);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["IpApi:BaseUrl"] = "https://ipapi.co"
            })
            .Build();

        var logger = new Mock<ILogger<IpGeolocationService>>();
        return new IpGeolocationService(factoryMock.Object, logger.Object, config);
    }

    [Fact]
    public async Task LookupIpAsync_ValidResponse_ReturnsResult()
    {
        var ipResult = new IpLookupResult
        {
            Ip          = "8.8.8.8",
            CountryCode = "US",
            CountryName = "United States",
            City        = "Mountain View",
            Region      = "California"
        };
        var json = JsonSerializer.Serialize(ipResult);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var service = BuildService(response);
        var result  = await service.LookupIpAsync("8.8.8.8");

        Assert.NotNull(result);
        Assert.Equal("US",            result!.CountryCode);
        Assert.Equal("United States", result.CountryName);
    }

    [Fact]
    public async Task LookupIpAsync_ApiReturnsNon200_ReturnsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        var service  = BuildService(response);
        var result   = await service.LookupIpAsync("8.8.8.8");

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupIpAsync_ApiReturnsErrorJson_ReturnsNull()
    {
        var errorJson = @"{""error"":true,""reason"":""RateLimited""}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(errorJson, Encoding.UTF8, "application/json")
        };

        var service = BuildService(response);
        var result  = await service.LookupIpAsync("8.8.8.8");

        Assert.Null(result);
    }
}
