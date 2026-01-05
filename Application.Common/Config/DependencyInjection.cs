using Application.Behaviors.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
            });

            services.AddApplicationBehaviors();
            return services;
        }
    }
}
