using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientConcurrencyLimit
{

    [Serializable]
    public class ConcurrencyLimitExceededException : ConcurrencyLimitException
    {
        public ConcurrencyLimitExceededException() { }
        public ConcurrencyLimitExceededException(string message) : base(message) { }
        public ConcurrencyLimitExceededException(string message, Exception inner) : base(message, inner) { }
        protected ConcurrencyLimitExceededException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
