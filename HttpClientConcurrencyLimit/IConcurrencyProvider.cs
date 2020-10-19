using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientConcurrencyLimit
{
    public interface IConcurrencyProvider
    {
        Task<ConcurrencyResult> BeginRequest(HttpRequestMessage request, LimitDetails limit, CancellationToken cancellationToken);
    }

    public class ConcurrencyResult
    {
        public static ConcurrencyResult RequestNotAllowed { get; } = new ConcurrencyResult(false);

        public bool RequestAllowed { get; }

        public ConcurrencyResult(bool requestAllowed)
        {
            RequestAllowed = requestAllowed;
        }

        public virtual Task EndRequest()
        {
            return Task.CompletedTask;
        }
    }
}
