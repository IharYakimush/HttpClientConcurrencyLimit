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
        private static readonly ConcurrentDictionary<ulong, SemaphoreSlim> Semaphores = new ConcurrentDictionary<ulong, SemaphoreSlim>();
        public async Task<ConcurrencyResult> BeginRequest(HttpRequestMessage request, LimitDetails limit, CancellationToken cancellationToken)
        {            
            SemaphoreSlim semaphore = Semaphores.GetOrAdd(limit.InProcId, (l) => new SemaphoreSlim(limit.MaxConcurrentRequets));

            TimeSpan timeout = limit.LimitExceededThrowImmidiately ? TimeSpan.Zero : limit.LimitExceededWaitTimeout;

            bool result = await semaphore.WaitAsync(timeout, cancellationToken);

            return new SemaphoreConcurrencyResult(result, semaphore);
        }
    }
}
