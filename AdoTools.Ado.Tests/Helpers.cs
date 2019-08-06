using NLog;
using NLog.Config;
using NLog.Targets;
using System.Threading.Tasks;
using AdoTools.Common.Entities;
using AdoTools.Ado.SourceTools;

namespace AdoTools.Ado.Tests
{
    public static class Helpers
    {
        public static void SetUpMemoryLogger(out Logger logger, out MemoryTarget warnTarget,
            out MemoryTarget traceTarget)
        {
            var configuration = new LoggingConfiguration();

            traceTarget = new MemoryTarget { Name = "trace" };
            warnTarget = new MemoryTarget { Name = "warn" };

            configuration.AddTarget(traceTarget);

            configuration.AddTarget(warnTarget);

            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, LogLevel.Warn, warnTarget));
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, LogLevel.Trace, traceTarget));

            LogManager.Configuration = configuration;

            logger = LogManager.GetLogger("test");
        }

        public static PerformanceConfiguration SetUpPerformanceConfiguration(
            int maxDegreeOfParallelism = 1, // default to single-threading to make debugging tests easier.
            ushort maximumRetries = 3,
            ushort retryDelay = 5
        )
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            var output = new PerformanceConfiguration(parallelOptions, maximumRetries, retryDelay);

            return output;
        }

        public static GitSourceTool SetUpFakeGitSourceTool(Logger logger = null, PerformanceConfiguration pc = null)
        {
            if (pc == null)
            {
                pc = SetUpPerformanceConfiguration();
            }

            var vsTsTool = SetUpFakeVsTsTool(pc);

            var sourceTool = new GitSourceTool(vsTsTool, logger, pc);

            return sourceTool;
        }

        public static VsTsTool SetUpFakeVsTsTool(PerformanceConfiguration pc = null, Logger logger = null)
        {
            var vsTsTool = new VsTsTool(
                TestData.TokenFake,
                TestData.OrganizationFake,
                TestData.ProjectFake,
                logger,
                pc ?? SetUpPerformanceConfiguration());
            return vsTsTool;
        }

        public static GitSourceTool SetUpRealGitSourceTool(Logger logger = null, PerformanceConfiguration pc = null)
        {
            var vsTsTool = SetUpRealVsTsTool();

            var output = new GitSourceTool(vsTsTool, logger, pc ?? SetUpPerformanceConfiguration());

            return output;
        }

        public static VsTsTool SetUpRealVsTsTool(Logger logger = null, PerformanceConfiguration pc = null)
        {
            var pat = AuthenticationHelper.GetPersonalAccessToken();
            var vsTsTool = new VsTsTool(pat, logger: logger, performanceConfiguration: pc ?? SetUpPerformanceConfiguration());

            return vsTsTool;
        }

        public static BuildTool SetUpRealBuildTool(out VsTsTool vsTsTool, PerformanceConfiguration pc = null)
        {
            if (pc == null)
            {
                pc = SetUpPerformanceConfiguration();
            }

            var pat = AuthenticationHelper.GetPersonalAccessToken();
            vsTsTool = new VsTsTool(pat, performanceConfiguration: pc);

            var output = new BuildTool(vsTsTool, performanceConfiguration: pc);

            return output;
        }

    }
}