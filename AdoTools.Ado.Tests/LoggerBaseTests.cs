using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace AdoTools.Ado.Tests
{
    [TestFixture]
    public static class LoggerBaseTests
    {
        private static void SetUpMemoryLogger(out Logger logger, out MemoryTarget warnTarget,
            out MemoryTarget traceTarget)
        {
            var configuration = new LoggingConfiguration();

            traceTarget = new MemoryTarget {Name = "trace"};
            warnTarget = new MemoryTarget {Name = "warn"};

            configuration.AddTarget(traceTarget);
            configuration.AddTarget(warnTarget);

            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, LogLevel.Warn, warnTarget));
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, LogLevel.Trace, traceTarget));

            LogManager.Configuration = configuration;

            logger = LogManager.GetLogger("test");
        }

        [Test]
        public static void Logger_Does_Not_Throw()
        {
            SetUpMemoryLogger(out var logger, out var warnTarget, out var traceTarget);

            Assert.Multiple(() =>
                {
                    Assert.That(() => logger.Fatal("Test Message"), Throws.Nothing);
                    Assert.That(() => logger.Error("Test Message"), Throws.Nothing);
                    Assert.That(() => logger.Warn("Test Message"), Throws.Nothing);
                    Assert.That(() => logger.Info("Test Message"), Throws.Nothing);
                    Assert.That(() => logger.Debug("Test Message"), Throws.Nothing);
                    Assert.That(() => logger.Trace("Test Message"), Throws.Nothing);

                    Assert.That(traceTarget.Logs.Count, Is.EqualTo(1), "Trace Count");
                    Assert.That(warnTarget.Logs.Count, Is.EqualTo(1), "Warn Count");
                }
            );
        }
    }
}