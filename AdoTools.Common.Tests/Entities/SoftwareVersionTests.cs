using System;
using NUnit.Framework;
using AdoTools.Common.Entities;

// ReSharper disable ObjectCreationAsStatement

namespace AdoTools.Common.Tests.Entities
{
    [TestFixture]
    public static class SoftwareVersionTests
    {
        [Test]
        public static void Constructor_Succeeds([Values("", "343", "xxx999")] string revision)
        {
            const int major = 1;
            const int minor = 2;
            const int build = 3;

            var input = $"{major}.{minor}.{build}";
            if (!string.IsNullOrWhiteSpace(revision))
            {
                input += $".{revision}";
            }

            Console.WriteLine($"Input: {input}");

            var result = new SoftwareVersion(input);

            Console.WriteLine($"Output: {result}");

            Assert.That(result, Is.Not.Null);

            Assert.That(result.Major, Is.EqualTo(major));
            Assert.That(result.Minor, Is.EqualTo(minor));
            Assert.That(result.Build, Is.EqualTo(build));
            Assert.That(result.Revision, Is.EqualTo(revision));

            Assert.That(result.ToString(), Is.EqualTo(input));
        }

        [Test]
        public static void Constructor_Throws([Values(null, "", "  ", "a.b.c.d", "a.b.c", "1.2.3.4.5", "1.2.3.dx23 ")]
            string input)
        {
            var exception = Assert.Throws<ArgumentException>(() => new SoftwareVersion(input));
            Console.WriteLine(exception.Message);
            StringAssert.Contains(
                string.IsNullOrWhiteSpace(input)
                    ? "Value must not be null or whitespace."
                    : "The version string must be in the format",
                exception.Message);
        }
    }
}