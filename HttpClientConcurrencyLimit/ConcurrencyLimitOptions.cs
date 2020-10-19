using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace HttpClientConcurrencyLimit
{
    public class ConcurrencyLimitOptions
    {
        public const int MaxSpecificLimits = short.MaxValue;

        private Dictionary<ushort, Tuple<LimitDetails, Func<HttpRequestMessage, bool>>> requestSpecificLimits = new Dictionary<ushort, Tuple<LimitDetails, Func<HttpRequestMessage, bool>>>();        

        public LimitDetails DefaultLimit { get; } = new LimitDetails();

        public bool IgnoreConcurrencyProviderExceptions { get; internal set; }

        /// <summary>
        /// Can be used by concurrency providers to isolate different projects in case of shared concurency limit info external storage. 
        /// </summary>
        public ushort ProjectId { get; set; } = 1;

        /// <summary>
        /// Can be used by concurrency providers to isolate concurency limit info for different HttpClients. Default value is a hash code from client name.
        /// </summary>
        public uint HttpClientId { get; set; }

        public ConcurrencyLimitOptions AddRequestSpecificLimit(LimitDetails limitDetails, Func<HttpRequestMessage, bool> matchConditions)
        {
            if (limitDetails == null) throw new ArgumentNullException(nameof(limitDetails));
            if (matchConditions == null) throw new ArgumentNullException(nameof(matchConditions));

            if (this.requestSpecificLimits.Count >= MaxSpecificLimits)
            {
                throw new InvalidOperationException($"Number of request specific limits should not exceed {MaxSpecificLimits}");
            }

            if (ReferenceEquals(DefaultLimit, limitDetails))
            {
                throw new InvalidOperationException("Default limit can't be used as request specific limit");
            }

            limitDetails = (LimitDetails)limitDetails.Clone();

            if (limitDetails.RequestSpecificId == 0)
            {
                for (ushort i = MaxSpecificLimits; i >= 1; i--)
                {
                    if (!this.requestSpecificLimits.ContainsKey(i))
                    {
                        limitDetails.RequestSpecificId = i;
                        break;
                    }
                }
            }

            if (this.requestSpecificLimits.ContainsKey(limitDetails.RequestSpecificId))
            {
                throw new InvalidOperationException($"Limit with id {limitDetails.RequestSpecificId} already added.");
            }
            else
            {
                this.requestSpecificLimits.Add(limitDetails.RequestSpecificId, new Tuple<LimitDetails, Func<HttpRequestMessage, bool>>(limitDetails, matchConditions));
            }

            return this;
        }

        public LimitDetails ResolveLimit(HttpRequestMessage message)
        {
            LimitDetails result = this.requestSpecificLimits.Values.FirstOrDefault(p => p.Item2(message))?.Item1;

            result = result ?? this.DefaultLimit;

            if (result.ProjectId == 0)
            {
                result.ProjectId = this.ProjectId;
            }

            if (result.HttpClientId == 0)
            {
                result.HttpClientId = this.HttpClientId;
            }

            return result;
        }
    }
}