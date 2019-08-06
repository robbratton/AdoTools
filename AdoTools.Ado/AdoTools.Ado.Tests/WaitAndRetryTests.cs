using System;
using System.Threading.Tasks;
using NLog.Targets;
using NUnit.Framework;
using Upmc.DevTools.Common.Entities;

namespace Upmc.DevTools.VsTs.Tests
{
    [TestFixture]
    public class WaitAndRetryBaseTests
    {
        private WaitAndRetryBase SetUpWaitAndRetryBase(ushort retries, out MemoryTarget warnTarget,
            out MemoryTarget traceTarget)
        {
            Helpers.SetUpMemoryLogger(out var logger, out warnTarget, out traceTarget);

            _performanceConfiguration =
                new PerformanceConfiguration(new ParallelOptions {MaxDegreeOfParallelism = 1}, retries, 1);

            return new WaitAndRetryBase(logger, _performanceConfiguration);
        }

        private const string Info = "Information";
        private PerformanceConfiguration _performanceConfiguration;

        [Test]
        public void HandlePolicyResult_Succeeds([Range(0, 3)] int retries)
        {
            var waitAndRetryBase = SetUpWaitAndRetryBase((ushort) retries, out var warnTarget, out var traceTarget);

            var result = waitAndRetryBase.WaitAndRetryPolicy.ExecuteAndCapture(
                context => context,
                WaitAndRetryBase.MakeContext(Info));

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => WaitAndRetryBase.HandlePolicyResult(result, Info), "Exception not Thrown");
                Assert.That(traceTarget.Logs.Count, Is.GreaterThan(0), "Trace Count");
                Assert.That(warnTarget.Logs.Count, Is.EqualTo(0), "Warn Count");
            });
        }

        [Test]
        public void HandlePolicyResult_Throws([Range(0, 2)] int retries)
        {
            var waitAndRetryBase = SetUpWaitAndRetryBase((ushort) retries, out var warnTarget, out var traceTarget);

            var result = waitAndRetryBase.WaitAndRetryPolicy.ExecuteAndCapture<object>(
                context => throw new TimeoutException(context.ToString()),
                WaitAndRetryBase.MakeContext(Info));

            Assert.Multiple(() =>
            {
                Assert.Throws<TimeoutException>(() => WaitAndRetryBase.HandlePolicyResult(result, Info),
                    "Exception Thrown");
                Assert.That(traceTarget.Logs.Count, Is.GreaterThan(0), "Trace Count");
                Assert.That(warnTarget.Logs.Count, Is.EqualTo(retries), "Warn Count");
            });
        }
    }
}