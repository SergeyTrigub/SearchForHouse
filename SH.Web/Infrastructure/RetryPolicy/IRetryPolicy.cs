using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SH.Web.Infrastructure.RetryPolicy
{
    public interface IRetryPolicy
    {
        void Retry(Func<object> operation, CancellationToken cancellationToken);

        Task RetryAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken);
    }
}