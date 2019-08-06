using System;
using System.Linq;
using NUnit.Framework;

namespace Upmc.DevTools.VsParser.Tests
{
    [TestFixture]
    public static class PackageConfigParserTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void ParsePackagesConfigFileContent_Throws(string input)
        {
            Assert.Throws<ArgumentException>(() => PackageConfigParser.ParsePackagesConfigFileContent(input));
        }

        [Test]
        public static void ParsePackagesConfigFileContent_ReturnsExpectedResult()
        {
            var content = Helpers.ReadEmbeddedResourceFile("packages.config.txt");

            var result = PackageConfigParser.ParsePackagesConfigFileContent(content).ToArray();

            Assert.That(result, Is.Not.Null);

            Assert.Multiple(
                () =>
                {
                    Assert.That(result.Length, Is.EqualTo(1));
                    Assert.That(result[0].Name, Is.EqualTo("Cake"));
                    Assert.That(result[0].Version, Is.EqualTo("0.17.0"));

                    // todo Do more asserts here.
                }
            );
        }
    }
}