using System;
using NLog;
using Polly;
using Polly.Retry;
using Upmc.DevTools.Common.Entities;

// ReSharper disable MemberCanBeProtected.Global

namespace Upmc.DevTools.VsTs
{
    /// <inheritdoc />
    /// <summary>
    ///     Base for all tools that use WaitAndRetry
    /// </summary>
    public class WaitAndRetryBase : LoggerBase
    {
        /*
         * For testing:
         * - Some of the methods and properties in this class are marked as internal instead of protected.
         * - The class can't be marked abstract.
         */

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional PerformanceConfigurations</param>
        protected internal WaitAndRetryBase(Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null) : base(logger)
        {
            Logger.Trace("Entering");

            _performanceConfiguration = performanceConfiguration ?? new PerformanceConfiguration();

            //Logger.Debug($"RetryWaitSeconds: {_performanceConfiguration.RetryDelay}");
            //Logger.Debug($"RetryCount: {_performanceConfiguration.MaximumRetries}");

            WaitAndRetryPolicyAsync = Policy
                .HandleInner<TimeoutException>()
                .WaitAndRetryAsync(
                    _performanceConfiguration.MaximumRetries,
                    SetDelay,
                    (response, delay, retryCount, context) => HandleRetry(response, retryCount, delay, context));

            WaitAndRetryPolicy = Policy
                .HandleInner<TimeoutException>()
                .WaitAndRetry(
                    _performanceConfiguration.MaximumRetries,
                    SetDelay,
                    (response, delay, retryCount, context) => HandleRetry(response, retryCount, delay, context));

            Logger.Trace("Exiting");
        }

        #endregion Constructors

        /*
         * Policy Configurations: https://www.visualstudio.com/en-us/docs/integrate/api/policy/configurations
         */

        #region Protected Methods

        /// <summary>
        ///     Prepares and throws an exception, if appropriate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">Policy Result</param>
        /// <param name="info">Information to be Added to the exception.</param>
        protected internal static void HandlePolicyResult<T>(PolicyResult<T> result, string info = "")
        {
            Logger.Trace("Entering");

            if (result.FinalException != null)
            {
                result.FinalException.Data.Add("info", info);
                result.FinalException.Data.Add("exceptionType", result.ExceptionType);
                result.FinalException.Data.Add("faultType", result.FaultType);

                Logger.Trace("Exiting");

                throw result.FinalException;
            }

            Logger.Trace("Exiting");
        }

        /// <summary>
        ///     Make a context for Polly
        /// </summary>
        /// <param name="info">Information to be reported in logs and exceptions</param>
        /// <returns></returns>
        protected internal static Context MakeContext(string info = "")
        {
            return new Context
            {
                {"info", info}
            };
        }

        // todo Move ExecuteAndCapture and ExecuteAndCapture here as generics somehow?

        #endregion Protected Methods

        #region Protected Properties and Fields

        /// <summary>
        ///     Policy for Wait and Retry - Async
        /// </summary>
        protected readonly AsyncRetryPolicy WaitAndRetryPolicyAsync;

        /// <summary>
        ///     Policy for Wait and Retry - Sync
        /// </summary>
        protected internal readonly RetryPolicy WaitAndRetryPolicy;

        /// <summary>
        ///     Performance Configuration
        /// </summary>
        private readonly PerformanceConfiguration _performanceConfiguration;

        #endregion Protected Properties and Fields

        #region Private Methods

        /// <summary>
        ///     Set the delay in a timespan for retries
        /// </summary>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        private TimeSpan SetDelay(int retryCount)
        {
            return TimeSpan.FromSeconds(_performanceConfiguration.RetryDelay * Math.Pow(2, retryCount));
        }

        /// <summary>
        ///     Handle a retry by generating a log message.
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        /// <param name="retryCount">This retry number</param>
        /// <param name="delay">Timespan representing the delay</param>
        /// <param name="context">The value for the info key will be used in the message.</param>
        private static void HandleRetry(Exception exception, int retryCount, TimeSpan delay, Context context)
        {
            Logger.Warn(exception,
                $"Retry occurred. RetryCount: {retryCount} Delay: {delay} {context["info"]}");
        }

        #endregion Private Methods
    }
}