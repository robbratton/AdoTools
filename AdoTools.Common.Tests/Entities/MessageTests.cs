using System;
using NUnit.Framework;
using AdoTools.Common.Tests.Helpers;

namespace AdoTools.Common.Tests.Entities
{
    [TestFixture]
    public static class MessageTests
    {
        private class TestClass
        {
            public TestClass(string left, string right)
            {
                Left = left;
                Right = right;
            }

            private string Left { get; }
            private string Right { get; }

            public override string ToString()
            {
                return $"{Left}--{Right}";
            }
        }

        [Test]
        public static void ToString_ReturnsExpectedResult([Values] bool hasIdentifier)
        {
            TestClass testClass = null;

            if (hasIdentifier)
            {
                testClass = new TestClass("x", "y");
            }

            var result = MessageGenerator<TestClass>.Generate(identifier: testClass);

            Assert.That(
                result.ToString(),
                hasIdentifier
                    ? Is.EqualTo($"{testClass} Warning Warning message")
                    : Is.EqualTo("Warning Warning message"));
        }
    }
}