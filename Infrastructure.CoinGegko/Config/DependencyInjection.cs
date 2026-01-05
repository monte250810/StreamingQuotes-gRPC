using Domain.Core.Interface.Infrastructure.Persistence;
using Infrastructure.CoinGegko.ExternalServices;
using Infrastructure.CoinGegko.ExternalServices.Interfaces;
using Infrastructure.CoinGegko.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Infrastructure.CoinGegko.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<CoinGeckoOptions>()
                .Bind(configuration.GetSection(CoinGeckoOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();


            services.AddOptions<ResilienceOptions>()
               .Bind(configuration.GetSection(ResilienceOptions.SectionName))
               .ValidateDataAnnotations()
               .ValidateOnStart();

            var coinGeckoOptions = configuration
             .GetSection(CoinGeckoOptions.SectionName)
             .Get<CoinGeckoOptions>() ?? new CoinGeckoOptions();

            var resilienceOptions = configuration
              .GetSection(ResilienceOptions.SectionName)
              .Get<ResilienceOptions>() ?? new ResilienceOptions();

            services.AddHttpClient<ICoinGeckoClient, CoinGeckoClient>(client =>
            {
                client.BaseAddress = new Uri(coinGeckoOptions.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(coinGeckoOptions.TimeoutSeconds);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "CryptoStreamingApp/1.0");

                if (!string.IsNullOrEmpty(coinGeckoOptions.ApiKey))
                {
                    client.DefaultRequestHeaders.Add("x-cg-demo-api-key", coinGeckoOptions.ApiKey);
                }
            })
            .AddResilienceHandler("CoinGecko", builder =>
            {
                builder
                    .AddRetry(new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = resilienceOptions.MaxRetries,
                        Delay = TimeSpan.FromMilliseconds(resilienceOptions.BaseDelayMs),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        ShouldHandle = args => ValueTask.FromResult(
                            args.Outcome.Result?.StatusCode is System.Net.HttpStatusCode.RequestTimeout
                            or System.Net.HttpStatusCode.ServiceUnavailable
                            or System.Net.HttpStatusCode.GatewayTimeout)
                    })
                    .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        FailureRatio = 0.5,
                        SamplingDuration = TimeSpan.FromSeconds(resilienceOptions.CircuitBreakerDurationSeconds),
                        MinimumThroughput = resilienceOptions.CircuitBreakerThreshold,
                        BreakDuration = TimeSpan.FromSeconds(resilienceOptions.CircuitBreakerDurationSeconds)
                    })
                    .AddTimeout(TimeSpan.FromMilliseconds(resilienceOptions.TimeoutMs));
            });

            services.AddTransient<ICryptoAssetProvider, CryptoAssetProvider>();
            return services;
        }
    }
}
