using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("AdoTools.Common.Tests")]

namespace AdoTools.Common
{
    /// <summary>
    ///     Methods to Run Constructors and Methods and Ignore Certain Exceptions
    /// </summary>
    /// <remarks>Consider using Polly (Policy) instead.</remarks>
    public static class IgnoreExceptionsHelper
    {
        /// <summary>
        ///     Instantiate a new instance given the parameter and ignore certain exceptions
        /// </summary>
        /// <typeparam name="TResult">Output type</typeparam>
        /// <param name="ignoreExceptions">Collection of except</param>
        /// <param name="input">Parameter values</param>
        /// <returns>
        ///     If no exception is thrown, returns the instantiated item.
        ///     If an ignored exception is thrown, returns the result type's default value.
        ///     If a non-ignored exception is thrown, rethrows it.
        /// </returns>
        /// <example>
        ///     // Call the constructor.
        ///     var result = IgnoreExceptionsHelper.DoConstructorIgnoringExceptions&lt;Uri&gt;(
        ///     new[] { typeof(UriFormatException) },
        ///     "junk uri");
        ///     // This will return null in the result variable because the method throws UriFormatException.
        /// </example>
        public static TResult DoConstructorIgnoringExceptions<TResult>(
            IEnumerable<Type> ignoreExceptions,
            params object[] input
        )
            where TResult : class
        {
            var ignoreExceptionsArray = ignoreExceptions as Type[] ?? ignoreExceptions.ToArray();
            if (ignoreExceptionsArray.Any(x => !x.IsSubclassOf(typeof(Exception)) && x != typeof(Exception)))
            {
                throw new ArgumentException("Value must be a list of types derived from Exception.",
                    nameof(ignoreExceptions));
            }

            var output = default(TResult);

            try
            {
                output = (TResult) Activator.CreateInstance(typeof(TResult), input);
            }
            catch (Exception exception)
            {
                CheckException(exception, ignoreExceptionsArray);
            }

            return output;
        }

        /// <summary>
        ///     DoMethodIgnoringExceptions the function and return null for certain exceptions
        /// </summary>
        /// <typeparam name="TResult">Return type</typeparam>
        /// <param name="theDelegate">Action to be performed.</param>
        /// <param name="ignoreExceptions">Collection of exceptions to be ignored</param>
        /// <param name="input">Parameter values</param>
        /// <returns>
        ///     If no exception is thrown, returns the result.
        ///     If an ignored exception is thrown, returns the result type's default value.
        ///     If a non-ignored exception is thrown, rethrows it.
        /// </returns>
        /// <example>
        ///     // Call the method. Make sure to set the return value type argument.
        ///     var result = IgnoreExceptionsHelper.DoMethodIgnoringExceptions&lt;string&gt;(
        ///     new Func&lt;string, string&gt;&lt;/string&gt;(File.ReadAllText);
        ///     new[] { typeof(FileNotFoundException) },
        ///     "junk filename");
        ///     // This will return null in the result variable because the method throws FileNotFoundException.
        /// </Example>
        public static TResult DoMethodIgnoringExceptions<TResult>(
            Delegate theDelegate,
            IEnumerable<Type> ignoreExceptions,
            params object[] input
        )
            where TResult : class
        {
            var ignoreExceptionsArray = ignoreExceptions as Type[] ?? ignoreExceptions.ToArray();
            if (ignoreExceptionsArray.Any(x => !x.IsSubclassOf(typeof(Exception)) && x != typeof(Exception)))
            {
                throw new ArgumentException("Value must be a list of types derived from Exception.",
                    nameof(ignoreExceptions));
            }

            var output = default(TResult);

            try
            {
                output = (TResult) theDelegate.DynamicInvoke(input);
            }
            catch (Exception exception)
            {
                CheckException(exception, ignoreExceptionsArray);
            }

            return output;
        }

        internal static void CheckException(
            Exception exception,
            Type[] ignoreExceptionsArray)
        {
            var targetInvocation = exception as TargetInvocationException;

            if (exception is AggregateException aggregate)
            {
                CheckInnerExceptions(aggregate.InnerExceptions, ignoreExceptionsArray);
            }
            else if (targetInvocation != null)
            {
                CheckException(targetInvocation.InnerException, ignoreExceptionsArray);
            }
            else
            {
                if (ignoreExceptionsArray.All(ignoreException => ignoreException != exception.GetType()))
                {
                    // This is not an ignored exception, so rethrow.
                    throw exception;
                }
            }
        }

        private static void CheckInnerExceptions(
            IEnumerable<Exception> exceptions,
            Type[] ignoreExceptionsArray)
        {
            foreach (var exception in exceptions)
            {
                CheckException(exception, ignoreExceptionsArray);
            }
        }
    }
}