using System;
using NUnit.Framework;

namespace AdoTools.Common.Tests
{
    [TestFixture]
    public static class ValidatorTests
    {
        [TestCase(123, false)]
        [TestCase(123.45, false)]
        [TestCase("abc", false)]
        [TestCase("", false)]
        [TestCase("    ", false)]
        [TestCase(null, true)]
        public static void AssertNotNull_ActsAppropriately(object input, bool throws)
        {
            if (throws)
            {
                Assert.That(() => Validators.AssertIsNotNull(input, nameof(input)), Throws.ArgumentException);
            }
            else
            {
                Assert.That(() => Validators.AssertIsNotNull(input, nameof(input)), Throws.Nothing);
            }
        }

        [TestCase("abc", false)]
        [TestCase("", true)]
        [TestCase("    ", false)]
        [TestCase(null, true)]
        public static void AssertNotNullOrEmpty_ActsAppropriately(string input, bool throws)
        {
            if (throws)
            {
                Assert.That(() => Validators.AssertIsNotNullOrEmpty(input, nameof(input)), Throws.ArgumentException);
            }
            else
            {
                Assert.That(() => Validators.AssertIsNotNullOrEmpty(input, nameof(input)), Throws.Nothing);
            }
        }

        [TestCase("abc", false)]
        [TestCase("", true)]
        [TestCase("    ", true)]
        [TestCase(null, true)]
        public static void AssertNotNullOrWhitespace_ActsAppropriately(string input, bool throws)
        {
            if (throws)
            {
                Assert.That(() => Validators.AssertIsNotNullOrWhitespace(input, nameof(input)), Throws.ArgumentException);
            }
            else
            {
                Assert.That(() => Validators.AssertIsNotNullOrWhitespace(input, nameof(input)), Throws.Nothing);
            }
        }
    }
}