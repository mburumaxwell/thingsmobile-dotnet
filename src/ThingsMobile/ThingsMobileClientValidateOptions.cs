using Microsoft.Extensions.Options;
using ThingsMobile;

namespace Microsoft.Extensions.DependencyInjection;

internal class ThingsMobileClientValidateOptions : IValidateOptions<ThingsMobileClientOptions>
{
    public ValidateOptionsResult Validate(string? name, ThingsMobileClientOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Username))
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.Username)}' must be provided.");
        }

        if (string.IsNullOrWhiteSpace(options.Token))
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.Token)}' must be provided.");
        }

        if (options.Endpoint == null)
        {
            return ValidateOptionsResult.Fail($"'{nameof(options.Endpoint)}' must be provided.");
        }

        return ValidateOptionsResult.Success;
    }
}
