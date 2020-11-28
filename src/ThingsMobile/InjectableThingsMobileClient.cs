using Microsoft.Extensions.Options;
using System.Net.Http;

namespace ThingsMobile
{
    /// <summary>
    /// A wrapped <see cref="ThingsMobileClient"/> with single constructor to inject an <see cref="HttpClient"/>
    /// whose lifetime is managed externally, e.g. by an DI container.
    /// </summary>
    internal class InjectableThingsMobileClient : ThingsMobileClient
    {
        public InjectableThingsMobileClient(HttpClient httpClient, IOptions<ThingsMobileClientOptions> optionsAccessor)
            : base(optionsAccessor.Value, httpClient) { }
    }
}
