using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace ThingsMobile.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void TestAddThingsMobileWithoutApiKey()
    {
        // Arrange
        var services = new ServiceCollection().AddThingsMobile(options => { }).Services.BuildServiceProvider();

        // Act && Assert
        Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<ThingsMobileClient>());
    }

    [Fact]
    public void TestAddThingsMobileReturnHttpClientBuilder()
    {
        // Arrange
        var collection = new ServiceCollection();

        // Act
        var builder = collection.AddThingsMobile(options =>
        {
            options.Username = "FAKE_USERNAME";
            options.Token = "FAKE_TOKEN";
        });

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<IHttpClientBuilder>(builder, exactMatch: false);
    }

    [Fact]
    public void TestAddThingsMobileRegisteredWithTransientLifeTime()
    {
        // Arrange
        var collection = new ServiceCollection();

        // Act
        var builder = collection.AddThingsMobile(options =>
        {
            options.Username = "FAKE_USERNAME";
            options.Token = "FAKE_TOKEN";
        });

        // Assert
        var serviceDescriptor = collection.FirstOrDefault(x => x.ServiceType == typeof(ThingsMobileClient));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Transient, serviceDescriptor!.Lifetime);
    }

    [Fact]
    public void TestAddThingsMobileCanResolveThingsMobileClientOptions()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddThingsMobile(options =>
            {
                options.Username = "FAKE_USERNAME";
                options.Token = "FAKE_TOKEN";
            }).Services.BuildServiceProvider();

        // Act
        var thingsMobileClientOptions = services.GetService<IOptions<ThingsMobileClientOptions>>();

        // Assert
        Assert.NotNull(thingsMobileClientOptions);
    }

    [Fact]
    public void TestAddThingsMobileCanResolveThingsMobileClient()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddThingsMobile(options =>
            {
                options.Username = "FAKE_USERNAME";
                options.Token = "FAKE_TOKEN";
            })
            .Services.BuildServiceProvider();

        // Act
        var thingsMobile = services.GetService<ThingsMobileClient>();

        // Assert
        Assert.NotNull(thingsMobile);
    }
}
