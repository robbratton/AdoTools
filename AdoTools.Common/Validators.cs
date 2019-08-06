using System;
// ReSharper disable UnusedMember.Global

namespace AdoTools.Common
{
    /// <summary>
    ///     Provides General Validators
    /// </summary>
    public static class Validators
    {
        /// <summary>
        ///     Assert that the input is not null.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void AssertIsNotNull(object input, string parameterName)
        {
            if (input == null)
            {
                throw new ArgumentException("Value must not be null.", parameterName);
            }
        }

        /// <summary>
        ///     Assert that the string input is not null or Empty.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void AssertIsNotNullOrEmpty(string input, string parameterName)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Value must not be null or empty.", parameterName);
            }
        }

        /// <summary>
        ///     Assert that the string input is not null.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void AssertIsNotNullOrWhitespace(string input, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Value must not be null or whitespace.", parameterName);
            }
        }
    }
}