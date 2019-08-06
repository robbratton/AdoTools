using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using AdoTools.Common;
using AdoTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.Ado.Tests.ToolTests
{
    [TestFixture]
    public static class NuGetToolTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void Constructor_Throws_WithBadRepoUri(string repoUri)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new NuGetTool(repoUri));
        }

        [TestCase(null, "password")]
        [TestCase("", "password")]
        [TestCase("  ", "password")]
        [TestCase("username", null)]
        [TestCase("username", "")]
        [TestCase("username", "  ")]
        public static void Constructor_Throws_WithMissingUsernameOrPassword(string username, string password)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new NuGetTool("https://www.google.com", false, username, password));
        }

        [Category("Integration")]
        //[TestCase("owin", true, false)] // owin will NOT work with a partial match
        //[TestCase("owin", false, true)] // owin will work with a partial match
        [TestCase(GoodNuGetPackageName, true, true)]
        [TestCase(NotFoundPackageName, true, false)]
        public static void Search_NuGetOrg_ReturnsExpectedResult(string packageName, bool exactMatch,
            bool expectedResult)
        {
            var tool = new NuGetTool();

            Assert.That(tool, Is.Not.Null);

            var result = tool.Search(packageName, exactMatch: exactMatch);

            Assert.That(result, Is.Not.Null);

            foreach (var item in result)
            {
                Console.WriteLine($"ID: {item.Identity.Id} Version: {item.Identity.Version}");
            }

            if (expectedResult)
            {
                Assert.That(result.Count(), Is.GreaterThan(0));
            }
            else
            {
                Assert.That(result.Count(), Is.EqualTo(0));
            }

            try
            {
                var myHandlers = new VariableDumpHandlers();
                Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Console.WriteLine(exception);
                // there is a loop somewhere in this object.
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void Search_Throws(string packageName)
        {
            var x = new NuGetTool();

            Assert.That(x, Is.Not.Null);

            Assert.That(() => x.Search(packageName), Throws.TypeOf<ArgumentException>());
        }

        [Category("Integration")]
        [TestCase(GoodNuGetPackageName, true)]
        [TestCase(NotFoundPackageName, false)]
        public static void PackageExists1_ReturnsExpectedValue(string packageName, bool expectedResult)
        {
            var tool = new NuGetTool();

            Assert.That(tool, Is.Not.Null);

            var result = tool.PackageExists(packageName);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Category("Integration")]
        [TestCase(GoodNuGetPackageName, GoodNuGetPackageVersion, true)]
        [TestCase(NotFoundPackageName, GoodNuGetPackageVersion, false)]
        [TestCase(GoodNuGetPackageName, BadNuGetPackageVersion, false)]
        [TestCase(NotFoundPackageName, BadNuGetPackageVersion, false)]
        public static void PackageExists2_ReturnsExpectedValue(string packageName, string version, bool expectedResult)
        {
            var tool = new NuGetTool();

            Assert.That(tool, Is.Not.Null);

            var result = tool.PackageExists(packageName, version);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void PackageExists_Throws(string packageName)
        {
            var tool = new NuGetTool();

            Assert.That(tool, Is.Not.Null);

            Assert.That(() => tool.PackageExists(packageName), Throws.TypeOf<ArgumentException>());
        }

        [Category("Integration")]
        [TestCase(GoodInternalPackageName, true)]
        [TestCase(NotFoundPackageName, false)]
        public static void Search_InternalRepo_ReturnsExpectedResult(string packageName, bool shouldBeFound)
        {
            var username = AuthenticationHelper.GetUsername();

            var pat = AuthenticationHelper.GetPersonalAccessToken();

            // todo Is there a new URI for this with dev.azure.com?
            var nuGetTool = new NuGetTool(
                "https://upmcappsvcs.pkgs.visualstudio.com/_packaging/services-common/nuget/v3/index.json",
                true,
                username,
                pat
            );

            Assert.That(nuGetTool, Is.Not.Null);

            var result = nuGetTool.Search(packageName);

            // ReSharper disable PossibleMultipleEnumeration
            Assert.That(result, Is.Not.Null);

            if (shouldBeFound)
            {
                // todo Check that multiple results were returned.
                Assert.That(result.Count(), Is.GreaterThan(2));
            }

            var firstSearchResult = result.FirstOrDefault();
            // ReSharper restore PossibleMultipleEnumeration

            if (shouldBeFound)
            {
                Assert.That(firstSearchResult, Is.Not.Null);
                Console.WriteLine(
                    $"Found ID: {firstSearchResult.Identity.Id} Version: {firstSearchResult.Identity.Version}");

                try
                {
                    Console.WriteLine(firstSearchResult.DumpValues(0));
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    Console.WriteLine(exception);
                    // there is a loop somewhere in this object.
                }
            }
            else
            {
                Assert.That(firstSearchResult, Is.Null);
            }
        }

        private const string NotFoundPackageName = "EA09BDB7-5248-4E20-98B4-87C68EA7CBE3";

        private const string GoodNuGetPackageName = "NUnit";
        private const string GoodNuGetPackageVersion = "3.11.0";
        private const string BadNuGetPackageVersion = "100.200.300";

        private const string GoodInternalPackageName = "Upmc.DevTools.Vsts";

        [Test]
        public static void Constructor_Succeeds()
        {
            var x = new NuGetTool();

            Assert.That(x, Is.Not.Null);
        }
    }
}