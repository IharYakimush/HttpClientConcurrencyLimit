using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientConcurrencyLimit
{
    internal class ConcurrencyLimitHandler : DelegatingHandler
    {
        public ConcurrencyLimitHandler(ConcurrencyLimitOptions options, IConcurrencyProvider concurrencyProvider)
        {
            Options = options ?? throw new System.ArgumentNullException(nameof(options));
            ConcurrencyProvider = concurrencyProvider ?? throw new System.ArgumentNullException(nameof(concurrencyProvider));
        }

        public ConcurrencyLimitOptions Options { get; }
        public IConcurrencyProvider ConcurrencyProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ConcurrencyProviderException">Exception during check wherever request not exceed limit using <see cref="IConcurrencyProvider.BeginRequest(HttpRequestMessage, LimitDetails, CancellationToken)"/></exception>
        /// <exception cref="ConcurrencyLimitExceededException">Concurrent requests limit exceeded and wait timeout not allowed or is over</exception>
        /// <exception cref="AggregateException">Wrap exceptions if they occured in both places (1. send request 2. mark request as ended in <see cref="ConcurrencyResult.EndRequest()"/>)</exception>        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LimitDetails limitDetails = this.Options.ResolveLimit(request);

            ConcurrencyResult concurrency;
            try
            {
                concurrency = await this.ConcurrencyProvider.BeginRequest(request, limitDetails, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exc)
            {
                if (this.Options.IgnoreConcurrencyProviderExceptions)
                {
                    return await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    throw new ConcurrencyProviderException("Unable to check concurrency limit. See inner exception for details", exc);
                }
            }

            if (concurrency.RequestAllowed)
            {
                Exception requestException = null;
                try
                {
                    return await base.SendAsync(request, cancellationToken);
                }      
                catch (Exception exc)
                {
                    requestException = exc;
                    throw;
                }
                finally
                {
                    try
                    {
                        await concurrency.EndRequest();
                    }
                    catch (Exception providerException)
                    {
                        if (!this.Options.IgnoreConcurrencyProviderExceptions)
                        {
                            if (requestException == null)
                            {
                                throw;
                            }
                            else
                            {
                                throw new AggregateException("Exception for send request and concurrency provider", requestException, providerException);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ConcurrencyLimitExceededException($"Concurrent requests limit exceeded. Details: {limitDetails}, State: {concurrency}");
            }
        }
    }
}