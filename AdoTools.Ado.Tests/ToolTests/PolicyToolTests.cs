using System;
using System.Linq;
using NUnit.Framework;
using AdoTools.Common;
using AdoTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.Ado.Tests.ToolTests
{
    [TestFixture]
    public static class PolicyToolTests
    {
        private static PolicyTool SetUpIntegration()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            var vsTsTool = new VsTsTool(pat);

            return new PolicyTool(vsTsTool);
        }

        [Test]
        public static void Constructor_Throws()
        {
            Assert.That(() => new PolicyTool(null), Throws.ArgumentException);
        }

        [Test]
        [Category("Integration")]
        public static void GetPolicyConfigurations_succeeds()
        {
            var policyTool = SetUpIntegration();

            var result = policyTool.GetPolicyConfiguration(1);

            Assert.That(result, Is.Not.Null);

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }

        [Test]
        [Category("Integration")]
        public static void ListPolicyConfigurations_succeeds()
        {
            var policyTool = SetUpIntegration();

            var result = policyTool.GetPolicyConfigurations();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }
    }
}