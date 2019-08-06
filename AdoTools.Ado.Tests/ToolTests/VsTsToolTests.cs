using System;
using System.Linq;
using NUnit.Framework;
using AdoTools.Common;
using AdoTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.Ado.Tests.ToolTests
{
    [TestFixture]
    public static class VsTsToolTests
    {
        // missing project
        [TestCase(TestData.OrganizationFake, TestData.TokenFake, null)]
        [TestCase(TestData.OrganizationFake, TestData.TokenFake, "")]
        [TestCase(TestData.OrganizationFake, TestData.TokenFake, "   ")]

        // missing token
        [TestCase(TestData.OrganizationFake, null, TestData.ProjectFake)]
        [TestCase(TestData.OrganizationFake, "", TestData.ProjectFake)]
        [TestCase(TestData.OrganizationFake, "   ", TestData.ProjectFake)]

        // missing organization
        [TestCase(null, TestData.TokenFake, TestData.ProjectFake)]
        public static void Constructor_Throws(string organization, string accessToken, string projectName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.That(() => new VsTsTool(accessToken, organization, projectName), Throws.ArgumentException);
        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        [TestCase("junkInput")]
        [TestCase("$This is a bad path")]
        public static void GetProjectName_Throws_WithBadInput(string input)
        {
            var result = Assert.Throws<ArgumentException>(() => VsTsTool.GetProjectNameFromPath(input));
            StringAssert.Contains(
                string.IsNullOrWhiteSpace(input)
                    ? "null or whitespace"
                    : "invalid TFS VC path",
                result.Message);
        }

        [Test]
        public static void Constructor_Succeeds()
        {
            const string org = "MyOrganization";

            var result = new VsTsTool("accessToken", org, "projectName");

            Assert.That(result.AzureDevOpsApiUri.ToString(), Is.EqualTo($"https://dev.azure.com/{org}"));
        }

        [Test]
        public static void DoHttpClientRequest_Throws()
        {
            var vsTsTool = new VsTsTool("JunkPat");

            Assert.That(() =>
                    vsTsTool.DoHttpClientRequest(null).Result,
                Throws.TypeOf<AggregateException>());
        }

        [Test]
        [Category("Integration")]
        public static void DoHttpClientRequest_TriesToAccessWeb()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            var vsTsTool = new VsTsTool(pat);

            var result = vsTsTool.DoHttpClientRequest("git/repositories").Result;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        [TestCase("$/Apollo/HP/Source/Dev/Monolith-HealthPlaNET/Branches/Dev/UPMC.Business", "Apollo")]
        [TestCase("$/Apollo/HP/Source/Tools/DevHTTPClientRepositories", "Apollo")]
        public static void GetProjectName_Succeeds(string input, string expectedOutput)
        {
            var result = VsTsTool.GetProjectNameFromPath(input);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [Category("Integration")]
        public static void GetProjects()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            var vsTsTool = new VsTsTool(pat);

            var result = vsTsTool.GetProjects();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }
    }
}