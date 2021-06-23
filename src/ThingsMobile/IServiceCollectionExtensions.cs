using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
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
        /// <param name="configuration">A configuration object with values for a <see cref="ThingsMobileClientOptions"/>.</param>
        /// <param name="configureOptions">A delegate that is used to configure a <see cref="ThingsMobileClientOptions"/>.</param>
        /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</returns>
        public static IHttpClientBuilder AddThingsMobile(this IServiceCollection services,
                                                               IConfiguration configuration = null,
                                                               Action<ThingsMobileClientOptions> configureOptions = null)
        {
            // if we have a configuration, add it
            if (configuration != null)
            {
                services.Configure<ThingsMobileClientOptions>(configuration);
            }

            // if we have a configuration action, add it
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            services
                 .PostConfigure<ThingsMobileClientOptions>(o =>
                 {
                     if (string.IsNullOrWhiteSpace(o.Username))
                     {
                         throw new ArgumentNullException(nameof(o.Username));
                     }

                     if (string.IsNullOrWhiteSpace(o.Token))
                     {
                         throw new ArgumentNullException(nameof(o.Token));
                     }

                     if (o.Endpoint == null)
                     {
                         throw new ArgumentNullException(nameof(o.Endpoint));
                     }

                 });

            services.TryAddTransient<ThingsMobileClient>(resolver => resolver.GetRequiredService<InjectableThingsMobileClient>());

            return services.AddHttpClient<InjectableThingsMobileClient>();
        }

        /// <summary>
        /// Adds the <see cref="IHttpClientFactory"/> with <see cref="ThingsMobileClient"/> and
        /// related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
        /// <param name="configureOptions">A delegate that is used to configure a <see cref="ThingsMobileClientOptions"/>.</param>
        /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</returns>
        public static IHttpClientBuilder AddThingsMobile(this IServiceCollection services,
                                                               Action<ThingsMobileClientOptions> configureOptions)
        {
            return services.AddThingsMobile(null, configureOptions);
        }

        /// <summary>
        /// Adds the <see cref="IHttpClientFactory"/> with <see cref="ThingsMobileClient"/> and
        /// related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> in which to register the services.</param>
        /// <param name="configuration">A configuration object with values for a <see cref="ThingsMobileClientOptions"/>.</param>
        /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</returns>
        public static IHttpClientBuilder AddThingsMobile(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddThingsMobile(configuration, null);
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
