using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientConcurrencyLimit
{
    public class ConcurrencyProviderSemaphoreSlim : IConcurrencyProvider
    {
        private static readonly ConcurrentDictionary<LimitDetails, SemaphoreSlim> Semaphores = new ConcurrentDictionary<LimitDetails, SemaphoreSlim>();
        public async Task<ConcurrencyResult> BeginRequest(HttpRequestMessage request, LimitDetails limit, CancellationToken cancellationToken)
        {
            SemaphoreSlim semaphore = Semaphores.GetOrAdd(limit, (l) => new SemaphoreSlim(l.MaxConcurrentRequets));

            TimeSpan timeout = limit.LimitExceededThrowImmidiately ? TimeSpan.Zero : limit.LimitExceededWaitTimeout;

            bool result = await semaphore.WaitAsync(timeout, cancellationToken);

            return new SemaphoreConcurrencyResult(result, semaphore);
        }
    }
}
