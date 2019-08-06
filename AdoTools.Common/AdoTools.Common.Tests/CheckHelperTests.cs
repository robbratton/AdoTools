using System;
using NUnit.Framework;
using Upmc.DevTools.Common.Entities;

namespace Upmc.DevTools.Common.Tests
{
    [TestFixture]
    public static class CheckHelperTests
    {
        [TestCase("aaa", CompareHelper.ComparisonOperator.EqualTo, "AAA", true, true)]
        [TestCase("aaa", CompareHelper.ComparisonOperator.EqualTo, "AAA", false, false)]
        public static void CheckSettingValueStringTest(
            string value,
            CompareHelper.ComparisonOperator comparisonOperator,
            string expectedValue,
            bool ignoreCase,
            bool expectedResult)
        {
            var result = CheckHelper<string>.CheckSettingValue(
                "SettingName",
                value,
                comparisonOperator,
                expectedValue,
                MessageLevel.Error,
                null,
                ignoreCase
            );

            Assert.That(result.MessageLevel, Is.EqualTo(expectedResult ? MessageLevel.Success : MessageLevel.Error));
        }

        [TestCase(1, CompareHelper.ComparisonOperator.EqualTo, 1, true, true)]
        [TestCase(1, CompareHelper.ComparisonOperator.EqualTo, 2, false, false)]
        public static void CheckSettingValueIntTest(
            int value,
            CompareHelper.ComparisonOperator comparisonOperator,
            int expectedValue,
            bool ignoreCase,
            bool expectedResult)
        {
            var result = CheckHelper<string>.CheckSettingValue(
                "SettingName",
                value,
                comparisonOperator,
                expectedValue,
                MessageLevel.Error,
                null,
                ignoreCase
            );

            Assert.That(result.MessageLevel, Is.EqualTo(expectedResult ? MessageLevel.Success : MessageLevel.Error));
        }

        [TestCase(1, CompareHelper.ComparisonOperator.EqualTo, 1, true, true, true)]
        [TestCase(1, CompareHelper.ComparisonOperator.EqualTo, 2, false, true, false)]
        [TestCase(1, CompareHelper.ComparisonOperator.EqualTo, null, false, true, false)]
        [TestCase(null, CompareHelper.ComparisonOperator.EqualTo, 1, false, false, true)]
        [TestCase(null, CompareHelper.ComparisonOperator.EqualTo, null, false, false, true)]
        public static void CheckSettingValueNullableIntTest(
            int? value,
            CompareHelper.ComparisonOperator comparisonOperator,
            int? expectedValue,
            bool ignoreCase,
            bool allowNull,
            bool expectedResult)
        {
            var result = CheckHelper<string>.CheckSettingValueNullable(
                "SettingName",
                value,
                comparisonOperator,
                expectedValue,
                allowNull,
                MessageLevel.Error,
                null,
                ignoreCase
            );

            Console.WriteLine(result);

            Assert.That(result.MessageLevel, Is.EqualTo(expectedResult ? MessageLevel.Success : MessageLevel.Error));
        }

    }


}
