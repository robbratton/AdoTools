using System;
using System.Reflection;
using AdoTools.Common.Entities;
// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global

namespace AdoTools.Common
{
    /// <summary>
    ///     Helper for running checks
    /// </summary>
    /// <typeparam name="T1">Message Identifier Type</typeparam>
    public static class CheckHelper<T1> where T1 : class
    {
        /// <summary>
        ///     Check a Nullable Setting's Value
        /// </summary>
        /// <typeparam name="T2">Value Type</typeparam>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <param name="operator"></param>
        /// <param name="expectedValue"></param>
        /// <param name="notNull"></param>
        /// <param name="failMessageLevel"></param>
        /// <param name="messageIdentifier"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static Message<T1> CheckSettingValueNullable<T2>(
            string settingName,
            T2? value,
            CompareHelper.ComparisonOperator @operator,
            T2? expectedValue,
            bool notNull = true,
            MessageLevel failMessageLevel = MessageLevel.Error,
            T1 messageIdentifier = null,
            bool ignoreCase = true
        )
            where T2 : struct
        {
            Message<T1> output;

            if (!value.HasValue || !expectedValue.HasValue)
            {
                if (notNull)
                {
                    output = new Message<T1>(failMessageLevel,
                        $"{settingName} is null. Expected {@operator} \"{expectedValue}\".", messageIdentifier);
                }
                else
                {
                    output = !expectedValue.HasValue
                        ? new Message<T1>(MessageLevel.Success, $"{settingName} nor expected value has a value.",
                            messageIdentifier)
                        : new Message<T1>(MessageLevel.Success,
                            $"{settingName} has no value. Not checking against expected value.", messageIdentifier);
                }
            }
            else
            {
                // Find the method.
                var method = typeof(CheckHelper<T1>).GetMethod(nameof(CheckSettingValue),
                    BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                {
                    throw new InvalidOperationException($"Could not find method {nameof(CheckSettingValue)}.");
                }

                // Make a Generic Version of the Method for the Type
                var genericMethod = method.MakeGenericMethod(typeof(T2));

                // Call the Generic Method.
                output = (Message<T1>) genericMethod.Invoke(null, new object[]
                {
                    settingName,
                    value.Value,
                    @operator,
                    expectedValue.Value,
                    failMessageLevel,
                    messageIdentifier,
                    ignoreCase
                });
            }

            return output;
        }

        /// <summary>
        ///     Check a Non-Nullable Setting's Value
        /// </summary>
        /// <typeparam name="T2">Value Type</typeparam>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <param name="operator"></param>
        /// <param name="expectedValue"></param>
        /// <param name="messageLevel"></param>
        /// <param name="messageIdentifier"></param>
        /// <param name="ignoreCase">(For strings only)</param>
        /// <returns></returns>
        public static Message<T1> CheckSettingValue<T2>(
            string settingName,
            T2 value,
            CompareHelper.ComparisonOperator @operator,
            T2 expectedValue,
            MessageLevel messageLevel = MessageLevel.Error,
            T1 messageIdentifier = null,
            bool ignoreCase = true
        )
            where T2 : IComparable 
        {
            int compareResult;

            // Special case for strings
            if (typeof(T2) == typeof(string))
            {
                var valueString = value as string;
                var expectedValueString = expectedValue as string;

                compareResult = string.Compare(valueString, expectedValueString, 
                    ignoreCase 
                        ? StringComparison.CurrentCultureIgnoreCase 
                        : StringComparison.Ordinal);
            }
            else {
                compareResult = value.CompareTo(expectedValue);
            }

            var output = CompareHelper.ComparisonIsTrue(@operator, compareResult)
                ? new Message<T1>(MessageLevel.Success, $"{settingName} is {@operator} \"{value}\" as expected.",
                    messageIdentifier)
                : new Message<T1>(
                    messageLevel,
                    $"{settingName} is \"{value}\". Expected {@operator} \"{expectedValue}\".", messageIdentifier);

            return output;
        }
    }
}