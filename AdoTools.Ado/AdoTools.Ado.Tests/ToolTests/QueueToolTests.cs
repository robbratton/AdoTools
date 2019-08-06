using System;
using System.Linq;
using NUnit.Framework;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Upmc.DevTools.VsTs.Tests.ToolTests
{
    [TestFixture]
    public static class QueueToolTests
    {
        private static QueueTool SetUpIntegration()
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();

            var vsTsTool = new VsTsTool(pat);

            return new QueueTool(vsTsTool);
        }

        [Test]
        public static void Constructor_Throws()
        {
            Assert.That(() => new QueueTool(null), Throws.ArgumentException);
        }

        [Test]
        [Category("Integration")]
        public static void GetQueue_succeeds()
        {
            var tool = SetUpIntegration();

            var result = tool.GetQueue(264);

            Assert.That(result, Is.Not.Null);

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }

        [Test]
        [Category("Integration")]
        public static void GetQueues_succeeds()
        {
            var tool = SetUpIntegration();

            var result = tool.GetQueues();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));

            var myHandlers = new VariableDumpHandlers();
            Console.WriteLine(result.DumpValues(0, handlers: myHandlers));
        }
    }
}