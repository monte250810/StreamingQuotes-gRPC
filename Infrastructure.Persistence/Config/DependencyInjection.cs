using Domain.Core.Interface.Infrastructure.Persistence;
using Infrastructure.Persistence.Dal;
using Infrastructure.Persistence.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<GrpcOptions>()
                .Bind(configuration.GetSection(GrpcOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}