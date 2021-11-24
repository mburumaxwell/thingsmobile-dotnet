using Microsoft.Extensions.Options;
using ThingsMobile;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IHttpClientFactory"/> with <see cref="ThingsMobileClient"/> and
        /// related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
        /// <param name="configure">A delegate that is used to configure a <see cref="ThingsMobileClientOptions"/>.</param>
        /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</returns>
        public static IHttpClientBuilder AddThingsMobile(this IServiceCollection services,
                                                         Action<ThingsMobileClientOptions>? configure = null)
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.AddSingleton<IValidateOptions<ThingsMobileClientOptions>, ThingsMobileClientValidateOptions>();

            return services.AddHttpClient<ThingsMobileClient>();
        }

        /// <summary>
        /// Adds the <see cref="IHttpClientFactory"/> with <see cref="ThingsMobileClient"/> and
        /// related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
        /// <param name="username">
        /// The username for the ThingsMobile account.
        /// This value maps to <see cref="ThingsMobileClientOptions.Username"/>
        /// </param>
        /// <param name="token">
        /// The token used to access the ThingsMobile APIs.
        /// This value maps to <see cref="ThingsMobileClientOptions.Token"/>
        /// </param>
        /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</returns>
        public static IHttpClientBuilder AddThingsMobile(this IServiceCollection services,
                                                         string username,
                                                         string token)
        {
            return services.AddThingsMobile(o =>
            {
                o.Username = username;
                o.Token = token;
            });
        }
    }
}
