using ThingsMobile;

var options = new ThingsMobileClientOptions
{
    Username = "your-username-here",
    Token = "your-token-here"
};
var client = new ThingsMobileClient(options);
var response = await client.ListSimCardsAsync();
var simcards = response.Resource;
foreach (var sim in simcards.Sims)
{
    Console.WriteLine($"MSISDN: {sim.Msisdn}");
    Console.WriteLine($"Name: {sim.Name}");
    Console.WriteLine($"Tag: {sim.Tag}");
    Console.WriteLine($"Status: {sim.Status}");
    Console.WriteLine("=====================");
}
