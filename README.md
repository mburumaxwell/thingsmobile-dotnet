# ThingsMobile

[![NuGet](https://img.shields.io/nuget/v/ThingsMobile.svg)](https://www.nuget.org/packages/ThingsMobile/)

## Introduction

ThingsMobile makes it easy to maintain SIM cards and their data plans/usage all over the world. More about ThingsMobile on the [website](https://thingsmobile.com).

The ThingsMobile dotnet NuGet package makes it easier to use the ThingsMobile API from your dotnet projects (netstandard2.0+) projects without having to build your own API calls. You can get your free API token at <https://www.thingsmobile.com/portal?step=api>.

The documentation that this Client is built on is available for download on the ThingsMobile portal <https://www.thingsmobile.com/portal?action=downloadApiDocument&uid=135421>.

> This library currently supports **v1.52 (2025-08-26)**

### Installation

To install using Package Manager Console use:
> Install-Package ThingsMobile

To install using dotnet cli use:
> dotnet add ThingsMobile

### Usage

```csharp
var options = new ThingsMobileClientOptions
{
    Username = "your-username-here",
    Token = "your-token-here"
};
var client = new ThingsMobileClient(options);
var response = await client.ListSimCardsAsync();
var simcards = response.Resource;
if (simcards is not null && simcards.Sims is not null)
{
    foreach (var sim in simcards.Sims)
    {
        Console.WriteLine($"MSISDN: {sim.Msisdn}");
        Console.WriteLine($"Name: {sim.Name}");
        Console.WriteLine($"Tag: {sim.Tag}");
        Console.WriteLine($"Status: {sim.Status}");
        Console.WriteLine("=====================");
    }
}
```

See [examples](./examples/) for more.

### Issues &amp; Comments

Please leave all comments, bugs, requests, and issues on the Issues page. We'll respond to your request ASAP!

### License

The Library is licensed under the [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form") license. Refer to the [LICENSE](./LICENSE) file for more information.
