using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientConcurrencyLimit
{
    class SemaphoreConcurrencyResult : ConcurrencyResult
    {
        private readonly SemaphoreSlim semaphore;

        public SemaphoreConcurrencyResult(bool requestAllowed, SemaphoreSlim semaphore) : base(requestAllowed)
        {
            this.semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
        }

        public override Task EndRequest()
        {
            semaphore.Release();

            return Task.CompletedTask;                 
        }
    }
}
