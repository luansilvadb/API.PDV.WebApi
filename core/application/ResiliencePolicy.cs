using System;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace API.PDV.Application
{
    public static class ResiliencePolicy
    {
        public static AsyncRetryPolicy CreateAsyncRetryPolicy(int retryCount, Action<Exception, int> onRetry)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount,
                    attempt => TimeSpan.FromMilliseconds(200 * attempt),
                    (exception, timeSpan, retry, context) => onRetry(exception, retry)
                );
        }

        public static async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action, LoggingService logger, int retryCount = 3)
        {
            var policy = CreateAsyncRetryPolicy(retryCount, (ex, retry) =>
            {
                logger.LogError(ex, $"Tentativa {retry} falhou. Retentando...");
            });

            return await policy.ExecuteAsync(action);
        }

        public static async Task ExecuteAsync(Func<Task> action, LoggingService logger, int retryCount = 3)
        {
            var policy = CreateAsyncRetryPolicy(retryCount, (ex, retry) =>
            {
                logger.LogError(ex, $"Tentativa {retry} falhou. Retentando...");
            });

            await policy.ExecuteAsync(action);
        }
    }
}