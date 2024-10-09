using AdoStateProcessor;
using AdoStateProcessor.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdoStateChangeHttpFunction
{
    public static class IOC
    {
        public static IServiceCollection AddAdoClientFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AdoOptions>()
                    .Bind(configuration.GetSection("AdoOptions"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IAdoFactory, AdoFactory>();
            return services;
        }
    }
}
