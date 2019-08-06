using System;
using System.Linq;
using NUnit.Framework;

namespace Upmc.DevTools.VsParser.Tests
{
    [TestFixture]
    public static class AssemblyInfoParserTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void ParseFileContent_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => AssemblyInfoParser.ParseFileContent(input));
        }

        [Test]
        public static void ParseFileContent_ReturnsExpectedResult()
        {
            var content = Helpers.ReadResourceFile("AssemblyInfo.cs.txt");

            var result = AssemblyInfoParser.ParseFileContent(content).ToArray();

            Assert.That(result, Is.Not.Null);

            Assert.Multiple(
                () =>
                {
                    Assert.That(result.Length, Is.EqualTo(7));

                    Assert.That(
                        result.FirstOrDefault(
                                x => x.Key.Equals(
                                    "System.Reflection.AssemblyFileVersionAttribute",
                                    StringComparison.CurrentCultureIgnoreCase))
                            .Value,
                        Is.EqualTo("1.2.3.4"));
                    Assert.That(
                        result.FirstOrDefault(
                                x => x.Key.Equals(
                                    "System.Reflection.AssemblyVersionAttribute",
                                    StringComparison.CurrentCultureIgnoreCase))
                            .Value,
                        Is.EqualTo("4.3.2.1"));
                }
            );
        }
    }
}