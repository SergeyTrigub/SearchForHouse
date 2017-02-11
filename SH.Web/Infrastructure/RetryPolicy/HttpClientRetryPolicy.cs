using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SH.Web.Infrastructure.RetryPolicy
{
    public class HttpClientRetryPolicy : IRetryPolicy
    {
        public int MaxRetryAttempts { get; set; } = 3;

        public int OperationDelaySeconds { get; set; } = 1;

        public void Retry(Func<object> operation, CancellationToken cancellationToken)
        {
            var attempts = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    attempts++;
                    HttpResponseMessage response = (HttpResponseMessage)operation();
                    if (response.IsSuccessStatusCode)
                    {
                        break; // Sucess! Lets exit the loop!
                    }
                    else
                    {
                        if ((int)response.StatusCode == 503
                            || (int)response.StatusCode == 429
                            || (int)response.StatusCode == 408
                            || (int)response.StatusCode == 520)
                        {
                            if (attempts == MaxRetryAttempts)
                                break;

                            Task.Delay(OperationDelaySeconds * 1000, cancellationToken).Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //logger.Error($"Exception caught on attempt {attempts} - will retry after delay {delay}", ex);
                    throw;
                }
            } while (true);
        }

        public async Task RetryAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            if (MaxRetryAttempts <= 0)
                throw new ArgumentOutOfRangeException(nameof(MaxRetryAttempts));

            var attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    HttpResponseMessage response = (await operation().ConfigureAwait(false)) as HttpResponseMessage;
                    if (response.IsSuccessStatusCode)
                    {
                        break; // Sucess! Lets exit the loop!
                    }
                    else
                    {
                        if ((int)response.StatusCode == 503
                            || (int)response.StatusCode == 429
                            || (int)response.StatusCode == 408
                            || (int)response.StatusCode == 520)
                        {
                            if (attempts == MaxRetryAttempts)
                                break;

                            await Task.Delay(OperationDelaySeconds * 1000, cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            } while (true);
        }
    }
}