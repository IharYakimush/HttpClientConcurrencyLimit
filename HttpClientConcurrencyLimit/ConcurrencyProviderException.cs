using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientConcurrencyLimit
{

    [Serializable]
    public class ConcurrencyProviderException : ConcurrencyLimitException
    {
        public ConcurrencyProviderException() { }
        public ConcurrencyProviderException(string message) : base(message) { }
        public ConcurrencyProviderException(string message, Exception inner) : base(message, inner) { }
        protected ConcurrencyProviderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
