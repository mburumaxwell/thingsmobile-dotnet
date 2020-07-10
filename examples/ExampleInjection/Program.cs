using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using ThingsMobile;

namespace ExampleInjection
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var username = "your-username-here";
            var token = "your-token-here";

            var services = new ServiceCollection();
            services.AddThingsMobile(username: username, token: token);

            var provider = services.BuildServiceProvider();

            var client = provider.GetRequiredService<ThingsMobileClient>();

            var response = await client.ListSimCardsAsync();
            var simcards = response.Resource;
            foreach (var sim in simcards.Sims)
            {
                Console.WriteLine($"MSISDN: {sim.Msisdn}, Name: {sim.Name}, Tag: {sim.Tag}, Status: {sim.Status}");
            }
        }
    }
}
