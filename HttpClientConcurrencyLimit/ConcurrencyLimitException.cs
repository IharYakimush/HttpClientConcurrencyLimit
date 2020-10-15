using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientConcurrencyLimit
{

    [Serializable]
    public abstract class ConcurrencyLimitException : Exception
    {
        public ConcurrencyLimitException() { }
        public ConcurrencyLimitException(string message) : base(message) { }
        public ConcurrencyLimitException(string message, Exception inner) : base(message, inner) { }
        protected ConcurrencyLimitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
