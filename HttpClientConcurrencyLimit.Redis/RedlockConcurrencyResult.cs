using RedLockNet;
using System;

using System.Threading.Tasks;

namespace HttpClientConcurrencyLimit.Redis
{
    class RedlockConcurrencyResult : ConcurrencyResult
    {
        internal event EventHandler OnEndRequest;

        public RedlockConcurrencyResult(IRedLock redLock) : base(redLock.IsAcquired)
        {
            RedLock = redLock ?? throw new ArgumentNullException(nameof(redLock));
        }

        public IRedLock RedLock { get; }

        public override Task EndRequest()
        {
            this.RedLock.Dispose();

            OnEndRequest?.Invoke(this, EventArgs.Empty);
            OnEndRequest = null;

            return Task.CompletedTask;
        }
    }
}
