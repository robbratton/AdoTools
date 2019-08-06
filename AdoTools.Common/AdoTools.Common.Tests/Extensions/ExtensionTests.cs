using System;
using NUnit.Framework;
using Upmc.DevTools.Common.Extensions;

namespace Upmc.DevTools.Common.Tests.Extensions
{
    [TestFixture]
    public static class ExtensionTests
    {
        [TestCase("This is a test.", "is a", true)]
        [TestCase("This is a test.", "IS A", true)]
        [TestCase("This is a test.", "not there", false)]
        public static void Contains_ReturnsExpectedResult(string test, string value, bool expectedResult)
        {
            Assert.That(test.ContainsInsensitive(value), Is.EqualTo(expectedResult));
        }
    }
}