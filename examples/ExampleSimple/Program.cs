using System;
using System.Threading.Tasks;
using ThingsMobile;

namespace ExampleSimple
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var options = new ThingsMobileClientOptions
            {
                Username = "your-username-here",
                Token = "your-token-here"
            };
            var client = new ThingsMobileClient(options);

            var response = await client.GetSimCardsLiteAsync();
            var simcards = response.Resource;
            if (simcards is not null && simcards.Sims is not null)
            {
                foreach (var sim in simcards.Sims)
                {
                    Console.WriteLine($"MSISDN: {sim.Msisdn}, Name: {sim.Name}, Tag: {sim.Tag}, Status: {sim.Status}");
                }
            }
        }
    }
}
