using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace HttpClientConcurrencyLimit
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddConcurrencyLimitation(this IHttpClientBuilder httpClientBuilder, IConcurrencyProvider provider, Action<ConcurrencyLimitOptions> optionsSetup = null)
        {
            if (httpClientBuilder is null)
            {
                throw new ArgumentNullException(nameof(httpClientBuilder));
            }

            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            ConcurrencyLimitOptions options = new ConcurrencyLimitOptions();
            optionsSetup?.Invoke(options);

            return httpClientBuilder.AddHttpMessageHandler(() => new ConcurrencyLimitHandler(options, provider));
        }

        public static IHttpClientBuilder AddConcurrencyLimitationSemaphoreSlim(this IHttpClientBuilder httpClientBuilder, Action<ConcurrencyLimitOptions> optionsSetup = null)
        {
            if (httpClientBuilder == null)
            {
                throw new ArgumentNullException(nameof(httpClientBuilder));
            }

            ConcurrencyLimitOptions options = new ConcurrencyLimitOptions();
            optionsSetup?.Invoke(options);

            return httpClientBuilder.AddHttpMessageHandler(() => new ConcurrencyLimitHandler(options, new ConcurrencyProviderSemaphoreSlim()));
        }
    }
}
