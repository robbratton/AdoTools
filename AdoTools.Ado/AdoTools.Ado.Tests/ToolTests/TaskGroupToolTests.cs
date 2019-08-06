using System;
using System.Linq;
using NUnit.Framework;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Upmc.DevTools.VsTs.Tests.ToolTests
{
    [TestFixture]
    public static class TaskGroupToolTests
    {
        private static TaskGroupTool SetUpIntegration()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            var vsTsTool = new VsTsTool(pat);

            return new TaskGroupTool(vsTsTool);
        }

        [Test]
        public static void Constructor_Throws()
        {
            Assert.That(() => new TaskGroupTool(null), Throws.ArgumentException);
        }

        [Test]
        [Category("Integration")]
        public static void GetTaskGroup_WithJunkID_Succeeds()
        {
            var taskGroupTool = SetUpIntegration();

            var result = taskGroupTool.GetTaskGroup(Guid.NewGuid());

            // Result will be null because we used a newly generated GUID.
            Assert.That(result, Is.Null);
        }

        [Test]
        [Category("Integration")]
        public static void GetTaskGroups_succeeds()
        {
            var taskGroupTool = SetUpIntegration();

            var result = taskGroupTool.GetTaskGroups();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));

            Console.WriteLine($"Found {result.Count()} taskGroupStep groups.");
            Console.WriteLine("");

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }
    }
}