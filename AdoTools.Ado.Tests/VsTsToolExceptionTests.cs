using System;
using FluentAssertions;
using NUnit.Framework;

namespace AdoTools.Ado.Tests
{
    [TestFixture]
    public static class VsTsToolExceptionTests
    {
        [Test]
        public static void Constructor1()
        {
            var result = new VsTsToolException();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.Not.Null);
        }

        [Test]
        public static void Constructor2()
        {
            const string message = "Test message.";

            var result = new VsTsToolException(message);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo(message));
        }

        [Test]
        public static void Constructor3()
        {
            const string message = "Test message.";
            var innerException = new InvalidOperationException();

            var result = new VsTsToolException(message, innerException);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo(message));
            result.InnerException.Should().BeEquivalentTo(innerException);
        }
    }
}