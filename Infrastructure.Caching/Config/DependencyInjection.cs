using Domain.Core.Interface;
using Infrastructure.Caching.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Caching.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureCaching(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<CachingOptions>()
                .Bind(configuration.GetSection(CachingOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();

            return services;
        }
    }
}
