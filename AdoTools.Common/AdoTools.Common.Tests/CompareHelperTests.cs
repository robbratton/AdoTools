using System;
using NUnit.Framework;
using Upmc.DevTools.Common.Entities;

namespace Upmc.DevTools.Common.Tests
{
    [TestFixture]
    public static class CompareHelperTests
    {
        [TestCase(CompareHelper.ComparisonOperator.EqualTo, 0, ExpectedResult = true)]
        [TestCase(CompareHelper.ComparisonOperator.GreaterThan, 1, ExpectedResult = true)]
        [TestCase(CompareHelper.ComparisonOperator.GreaterThanOrEqualTo, 0, ExpectedResult = true)]
        [TestCase(CompareHelper.ComparisonOperator.GreaterThanOrEqualTo, 1, ExpectedResult = true)]
        [TestCase(CompareHelper.ComparisonOperator.LessThan, -1, ExpectedResult = true)]
        [TestCase(CompareHelper.ComparisonOperator.LessThanOrEqualTo, 0, ExpectedResult = true)]
        [TestCase(CompareHelper.ComparisonOperator.LessThanOrEqualTo, -1, ExpectedResult = true)]
        [TestCase(CompareHelper.ComparisonOperator.EqualTo, -1, ExpectedResult = false)]
        [TestCase(CompareHelper.ComparisonOperator.EqualTo, 1, ExpectedResult = false)]
        [TestCase(CompareHelper.ComparisonOperator.GreaterThan, 0, ExpectedResult = false)]
        [TestCase(CompareHelper.ComparisonOperator.GreaterThan, -1, ExpectedResult = false)]
        [TestCase(CompareHelper.ComparisonOperator.GreaterThanOrEqualTo, -1, ExpectedResult = false)]
        [TestCase(CompareHelper.ComparisonOperator.LessThan, 0, ExpectedResult = false)]
        [TestCase(CompareHelper.ComparisonOperator.LessThan, 1, ExpectedResult = false)]
        [TestCase(CompareHelper.ComparisonOperator.LessThanOrEqualTo, 1, ExpectedResult = false)]
        public static bool ComparisonIsTrue_ReturnsExpectedValue(CompareHelper.ComparisonOperator comparison, int value)
        {
            return CompareHelper.ComparisonIsTrue(comparison, value);
        }

        /// <summary>
        ///     Tests BOTH CheckSettingValueNullable and CheckSettingValue.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operator"></param>
        /// <param name="expected"></param>
        /// <param name="notNull"></param>
        /// <returns></returns>
        [TestCase(1, CompareHelper.ComparisonOperator.EqualTo, 1, true, ExpectedResult = MessageLevel.Success)]
        [TestCase(1, CompareHelper.ComparisonOperator.EqualTo, 0, true, ExpectedResult = MessageLevel.Error)]
        [TestCase(null, CompareHelper.ComparisonOperator.EqualTo, null, true, ExpectedResult = MessageLevel.Error)]
        [TestCase(null, CompareHelper.ComparisonOperator.EqualTo, null, false, ExpectedResult = MessageLevel.Success)]
        [TestCase(null, CompareHelper.ComparisonOperator.EqualTo, 1, false, ExpectedResult = MessageLevel.Success)]
        public static MessageLevel CheckSettingValueNullable_ReturnsExpectedResult(int? input,
            CompareHelper.ComparisonOperator @operator, int? expected, bool notNull)
        {
            const string settingName = "settingName";

            var result =
                CheckHelper<object>.CheckSettingValueNullable(settingName, input, @operator, expected, notNull);

            Assert.That(result, Is.Not.Null);

            return result.MessageLevel;
        }
    }
}