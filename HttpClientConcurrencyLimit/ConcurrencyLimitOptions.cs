using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace HttpClientConcurrencyLimit
{
    public class ConcurrencyLimitOptions
    {
        private LinkedList<Tuple<LimitDetails, Func<HttpRequestMessage, bool>>> requestSpecificLimits = new LinkedList<Tuple<LimitDetails, Func<HttpRequestMessage, bool>>>();

        public LimitDetails DefaultLimit = new LimitDetails();

        public bool IgnoreConcurrencyProviderExceptions { get; internal set; }

        public ConcurrencyLimitOptions AddRequestSpecificLimit(LimitDetails limitDetails, Func<HttpRequestMessage, bool> matchConditions)
        {
            if (limitDetails == null) throw new ArgumentNullException(nameof(limitDetails));
            if (matchConditions == null) throw new ArgumentNullException(nameof(matchConditions));

            this.requestSpecificLimits.AddLast(new Tuple<LimitDetails, Func<HttpRequestMessage, bool>>(limitDetails, matchConditions));

            return this;
        }

        public LimitDetails ResolveLimit(HttpRequestMessage message)
        {
            LimitDetails result = this.requestSpecificLimits.FirstOrDefault(p => p.Item2(message))?.Item1;

            return result ?? this.DefaultLimit;
        }
    }
}