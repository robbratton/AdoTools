using System;
using System.Linq;
using NUnit.Framework;
using AdoTools.Common;
using AdoTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.Ado.Tests.ToolTests
{
    [TestFixture]
    public static class VariableGroupToolTests
    {
        [TestCase("Token-Defaults", true)]
        [TestCase("This is not a real name!", false)]
        [Category("Integration")]
        public static void GetGroupID_succeeds(string name, bool expectFound)
        {
            var variableGroupTool = SetUpIntegration();

            if (expectFound)
            {
                var result = variableGroupTool.GetVariableGroupId(name);
                Assert.That(result, Is.Not.Null);
            }
            else
            {
                Assert.That(
                    () => variableGroupTool.GetVariableGroupId(name),
                    Throws.InvalidOperationException
                        .With.Message.EqualTo("Sequence contains no matching element")
                );
            }
        }

        private static VariableGroupTool SetUpIntegration()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            var vsTsTool = new VsTsTool(pat);

            return new VariableGroupTool(vsTsTool);
        }

        [Test]
        public static void Constructor_Throws()
        {
            Assert.That(() => new VariableGroupTool(null), Throws.ArgumentException);
        }

        [Test]
        [Category("Integration")]
        public static void GetVariableGroupNames_succeeds()
        {
            var variableGroupTool = SetUpIntegration();

            var result = variableGroupTool.GetVariableGroupNames();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }

        [Test]
        [Category("Integration")]
        public static void GetVariableGroups_succeeds()
        {
            var variableGroupTool = SetUpIntegration();

            var result = variableGroupTool.GetVariableGroups();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));

            Console.WriteLine(result.DumpValues(0));
        }

        [Test]
        [Category("Integration")]
        public static void GetVariableNames_succeeds()
        {
            var variableGroupTool = SetUpIntegration();

            var groupId = variableGroupTool.GetVariableGroupId("Token-Defaults");

            var result = variableGroupTool.GetVariableNames(groupId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));

            Console.WriteLine(result.DumpValues(0));
        }
    }
}