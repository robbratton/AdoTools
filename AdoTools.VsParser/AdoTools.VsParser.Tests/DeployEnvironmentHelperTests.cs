using System;
using NUnit.Framework;
using Upmc.DevTools.VsParser.Entities;

namespace Upmc.DevTools.VsParser.Tests
{
    [TestFixture]
    public static class DeployEnvironmentHelperTests
    {
        [TestCase("debug", DeployEnvironment.None)]
        [TestCase("dev", DeployEnvironment.Development)]
        [TestCase("development", DeployEnvironment.Development)]
        [TestCase("lab", DeployEnvironment.Lab)]
        [TestCase("Prod", DeployEnvironment.Production)]
        [TestCase("prodSupport", DeployEnvironment.ProdSupport)]
        [TestCase("QA", DeployEnvironment.Qa)]
        [TestCase("Training", DeployEnvironment.Training)]
        [TestCase("Release", DeployEnvironment.None)]
        public static void MapEnvironment_ReturnsExpectedResult(string input, DeployEnvironment expectedOutput)
        {
            var result = DeployEnvironmentHelper.Map(input, true);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [TestCase("Junk string")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void MapEnvironment_Throws(string input)
        {
            Assert.That(() => DeployEnvironmentHelper.Map(input, true), Throws.ArgumentException);
        }

        [TestCase("Junk string")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void MapEnvironment_ReturnsNone(string input)
        {
            var result = DeployEnvironmentHelper.Map(input);

            Assert.That(result, Is.EqualTo(DeployEnvironment.None));
        }
    }
}