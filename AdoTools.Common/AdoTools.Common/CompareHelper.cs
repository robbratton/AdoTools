using System;

namespace Upmc.DevTools.Common
{
    /// <summary>
    ///     Tools for Comparing Values and Generating User-Friendly Messages
    /// </summary>
    public static class CompareHelper
    {
        /// <summary>
        ///     Comparison Operators
        /// </summary>
        public enum ComparisonOperator
        {
            /// <summary>
            ///     Left Is Less Than Right
            /// </summary>
            LessThan,

            /// <summary>
            ///     Left is Less Than or Equal to Right
            /// </summary>
            LessThanOrEqualTo,

            /// <summary>
            ///     Left Is Equal to Right
            /// </summary>
            EqualTo,

            /// <summary>
            ///     Left is Greater Than or Equal to Right
            /// </summary>
            GreaterThanOrEqualTo,

            /// <summary>
            ///     Left is Greater Than Right
            /// </summary>
            GreaterThan
        }

        /// <summary>
        ///     Indicates if the comparison is true for the given operator.
        /// </summary>
        /// <param name="operator">The Operator that was Applied</param>
        /// <param name="compareResult">Output of the CompareTo Method</param>
        /// <returns>True if the operator's result passed.</returns>
        public static bool ComparisonIsTrue(ComparisonOperator @operator, int compareResult)
        {
            // ReSharper disable ArrangeRedundantParentheses
            var output =
                    (@operator == ComparisonOperator.LessThan && compareResult == -1)
                    ||
                    (@operator == ComparisonOperator.LessThanOrEqualTo && compareResult <= 0)
                    ||
                    (@operator == ComparisonOperator.EqualTo && compareResult == 0)
                    ||
                    (@operator == ComparisonOperator.GreaterThanOrEqualTo && compareResult >= 0)
                    ||
                    (@operator == ComparisonOperator.GreaterThan && compareResult == 1)
                ;
            // ReSharper restore ArrangeRedundantParentheses

            return output;
        }
    }
}