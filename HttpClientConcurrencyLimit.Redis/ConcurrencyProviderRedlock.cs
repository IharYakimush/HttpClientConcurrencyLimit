using RedLockNet;
using RedLockNet.SERedis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientConcurrencyLimit.Redis
{
    public class ConcurrencyProviderRedlock : IConcurrencyProvider
    {            
        private static Stopwatch stopwatch = new Stopwatch();

        private static ConcurrentDictionary<LimitDetails, long> lastTiming = new ConcurrentDictionary<LimitDetails, long>();

        static ConcurrencyProviderRedlock()
        {
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
            }
        }

        public ConcurrencyProviderRedlock(RedLockFactory redLockFactory)
        {
            RedLockFactory = redLockFactory ?? throw new ArgumentNullException(nameof(redLockFactory));            
        }

        public RedLockFactory RedLockFactory { get; }        

        public async Task<ConcurrencyResult> BeginRequest(HttpRequestMessage request, LimitDetails limit, CancellationToken cancellationToken)
        {
            long start = stopwatch.ElapsedMilliseconds;
            long end = start + (long)limit.LimitExceededWaitTimeout.TotalMilliseconds + 1;
            while (true)
            {
                long currentBegin = stopwatch.ElapsedMilliseconds;

                for (int i = 0; i < limit.MaxConcurrentRequets; i++)
                {
                    IRedLock rl = await this.RedLockFactory.CreateLockAsync($"p{limit.ProjectId}_c{limit.HttpClientId}_r{limit.RequestSpecificId}_n{i}", TimeSpan.FromMinutes(1));

                    if (rl.IsAcquired)
                    {
                        long lockAt = stopwatch.ElapsedMilliseconds;

                        RedlockConcurrencyResult result = new RedlockConcurrencyResult(rl);

                        result.OnEndRequest += (o, e) => { lastTiming.AddOrUpdate(limit, stopwatch.ElapsedMilliseconds - lockAt, (l, c) => stopwatch.ElapsedMilliseconds - lockAt); };

                        return result;
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }

                long currentEnd = stopwatch.ElapsedMilliseconds;

                if (limit.LimitExceededThrowImmidiately || currentEnd >= end)
                {
                    return ConcurrencyResult.RequestNotAllowed;
                }

                long statistic = lastTiming.GetOrAdd(limit, 5000) / limit.MaxConcurrentRequets;
                long statisticWithLimits = Math.Max(1000, Math.Min(15000, statistic));
                long timeBeforeWaitTimeout = end - currentEnd;
                long timeToTryAllLocks = currentEnd - currentBegin;

                long timeout = Math.Min(timeBeforeWaitTimeout, Math.Max(timeToTryAllLocks, statisticWithLimits));

                await Task.Delay((int)timeout, cancellationToken);
            }
        }
    }
}
