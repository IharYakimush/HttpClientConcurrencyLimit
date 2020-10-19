using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientConcurrencyLimit
{
    public class LimitDetails : ICloneable
    {
        public int MaxConcurrentRequets { get; set; } = 1;

        public bool LimitExceededThrowImmidiately { get; set; } = false;

        public TimeSpan LimitExceededWaitTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public ushort RequestSpecificId { get; set; } = 0;

        public ushort ProjectId { get; set; } = 0;

        public uint HttpClientId { get; set; } = 0;

        public ulong InProcId => this.HttpClientId * (ulong)100000 + this.RequestSpecificId;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"if limit of {MaxConcurrentRequets} concurrent requets exceeded throw " + (LimitExceededThrowImmidiately ? " immidiately" : $"after {LimitExceededWaitTimeout}");
        }
    }
}
