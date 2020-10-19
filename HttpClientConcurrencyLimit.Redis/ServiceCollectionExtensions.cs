using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using RedLockNet;
using RedLockNet.SERedis;

namespace HttpClientConcurrencyLimit.Redis
{
    public static class ServiceCollectionExtensions
    {        
        public static IHttpClientBuilder AddConcurrencyLimitationRedLock(this IHttpClientBuilder httpClientBuilder, RedLockFactory redLockFactory, Action<ConcurrencyLimitOptions> optionsSetup = null)
        {
            return httpClientBuilder.AddConcurrencyLimitation(new ConcurrencyProviderRedlock(redLockFactory), optionsSetup);
        }

        public static IHttpClientBuilder AddConcurrencyLimitationRedLock(this IHttpClientBuilder httpClientBuilder, Action<ConcurrencyLimitOptions> optionsSetup = null)
        {
            return httpClientBuilder.AddConcurrencyLimitation((sp) => new ConcurrencyProviderRedlock(sp.GetRequiredService<RedLockFactory>()), optionsSetup);
        }
    }
}
