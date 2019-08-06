using System;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace Upmc.DevTools.Common.Entities
{
    /// <summary>
    ///     Performance Configuration Options for Parallelism and Retries
    /// </summary>
    public class PerformanceConfiguration
    {
        /// <summary>
        ///     Maximum Retries
        /// </summary>
        public readonly ushort MaximumRetries;

        /// <summary>
        ///     Parallel Options
        /// </summary>
        public readonly ParallelOptions ParallelOptions;

        /// <summary>
        ///     Retry Delay
        /// </summary>
        public readonly ushort RetryDelay;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="parallelOptions">Options for Parallel Operations</param>
        /// <param name="maximumRetries">Maximum Number of Retries</param>
        /// <param name="retryDelay">Delay in Seconds Between Retries, Exponential</param>
        public PerformanceConfiguration(
            ParallelOptions parallelOptions = null,
            ushort maximumRetries = 5,
            ushort retryDelay = 10)
        {
            ParallelOptions = parallelOptions ?? new ParallelOptions();
            MaximumRetries = maximumRetries;
            RetryDelay = retryDelay;
        }
    }
}