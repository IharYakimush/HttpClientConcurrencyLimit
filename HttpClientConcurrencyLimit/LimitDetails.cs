using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientConcurrencyLimit
{
    public class LimitDetails
    {
        public int MaxConcurrentRequets { get; set; } = 1;

        public bool LimitExceededThrowImmidiately { get; set; } = false;

        public TimeSpan LimitExceededWaitTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public override string ToString()
        {
            return $"if limit of {MaxConcurrentRequets} concurrent requets exceeded throw " + (LimitExceededThrowImmidiately ? " immidiately" : $"after {LimitExceededWaitTimeout}");
        }
    }
}
