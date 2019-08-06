using System;
using NUnit.Framework;
using Upmc.DevTools.Common.Extensions;

namespace Upmc.DevTools.Common.Tests.Extensions
{
    [TestFixture]
    public static class ContainsExtensionsTests
    {
        [TestCase("This is a test", "IS A", StringComparison.CurrentCultureIgnoreCase, true)]
        [TestCase("This is a test", "IS A", StringComparison.Ordinal, false)]
        public static void ContainsInsensitive_ReturnsExpectedResult(
            string input,
            string content,
            StringComparison stringComparison,
            bool expectedResult
        )
        {
            var result = input.ContainsInsensitive(content, stringComparison);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}