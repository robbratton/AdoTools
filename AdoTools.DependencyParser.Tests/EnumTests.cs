using System;
using NUnit.Framework;
using AdoTools.DependencyParser.Entities;

namespace AdoTools.DependencyParser.Tests
{
    [TestFixture]
    public class EnumTests : TestBase
    {
        [TestCase("debug", DeployEnvironment.None)]
        [TestCase("dev", DeployEnvironment.Development)]
        [TestCase("development", DeployEnvironment.Development)]
        [TestCase("LAB", DeployEnvironment.Lab)]
        [TestCase("prod", DeployEnvironment.Production)]
        // ReSharper disable once StringLiteralTypo
        [TestCase("prodsupport", DeployEnvironment.ProdSupport)]
        [TestCase("qa", DeployEnvironment.Qa)]
        [TestCase("training", DeployEnvironment.Training)]
        public static void MapEnvironment_ReturnsExpectedResult(string path, DeployEnvironment expectedResult)
        {
            var result = DeployEnvironmentHelper.MapEnvironment(path);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("Junk1")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void MapEnvironment_Throws(string path)
        {
            Assert.That(() => DeployEnvironmentHelper.MapEnvironment(path), Throws.ArgumentException);
        }

        [TestCase("a.sln", FileType.Solution)]
        // ReSharper disable once StringLiteralTypo
        [TestCase("a.sqlproj", FileType.SqlProject)]
        [TestCase("app.config", FileType.AppWebDataConfig)]
        [TestCase("web.config", FileType.AppWebDataConfig)]
        // ReSharper disable once StringLiteralTypo
        [TestCase("dataconfiguration.config", FileType.AppWebDataConfig)]
        [TestCase("appsettings.json", FileType.AppsettingsJson)]
        [TestCase("x.csproj", FileType.CsProject)]
        // ReSharper disable once StringLiteralTypo
        [TestCase("assemblyinfo.cs", FileType.AssemblyInfo)]
        [TestCase("misc.cs", FileType.Unknown)]
        [TestCase("junk", FileType.Unknown)]
        [TestCase(".ext", FileType.Unknown)]
        [TestCase(".", FileType.Unknown)]
        [TestCase("packages.config", FileType.PackageConfig)]
        public static void MapType_ReturnsExpectedResult(string path, FileType expectedResult)
        {
            var result = FileTypeHelper.MapType(path);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("   ")]
        [TestCase("")]
        [TestCase(null)]
        public static void MapType_Throws(string path)
        {
            Assert.That(() => FileTypeHelper.MapType(path), Throws.ArgumentException);
        }
    }
}