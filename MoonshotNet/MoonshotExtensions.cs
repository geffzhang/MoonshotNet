using MoonshotNet.ChatCompletions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MoonshotNet
{
    public static class MoonshotExtensions
    {
        public static void AddMoonshotClient(
            this IServiceCollection services,
            string apiKey,
            MoonshotModel model,
            string? serviceId = null)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
            ArgumentNullException.ThrowIfNull(model);

            Func<IServiceProvider, object?, MoonshotClient> factory = (serviceProvider, _) =>
                new(apiKey,
                    model,
                    serviceProvider?.GetService<HttpClient>(),
                    serviceProvider?.GetService<ILogger<MoonshotClient>>());

            services.AddKeyedSingleton(serviceId, factory);
        }
    }
}
