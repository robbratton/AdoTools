using NUnit.Framework;
using System.Threading.Tasks;
using AdoTools.Common.Entities;

namespace AdoTools.Common.Tests.Entities
{
    [TestFixture]
    public static class PerformanceConfigurationTests
    {
        [Test]
        public static void Constructor3_Succeeds(
            [Values] bool includeParallelOptions,
            [Values((ushort)0, (ushort)1)] ushort maximumRetries,
            [Values((ushort)0, (ushort)1)] ushort retryDelay)
        {
            ParallelOptions parallelOptions = null;

            if (includeParallelOptions)
            {
                parallelOptions = new ParallelOptions();
            }

            Assert.That(() => new PerformanceConfiguration(parallelOptions, maximumRetries, retryDelay),
                Throws.Nothing);
        }

        [Test]
        public static void Constructor2_Succeeds(
            [Values] bool includeParallelOptions,
            [Values((ushort)0, (ushort)1)] ushort maximumRetries
        )
        {
            ParallelOptions parallelOptions = null;

            if (includeParallelOptions)
            {
                parallelOptions = new ParallelOptions();
            }

            Assert.That(() => new PerformanceConfiguration(parallelOptions, maximumRetries),
                Throws.Nothing);
        }

        [Test]
        public static void Constructor_Succeeds(
            [Values] bool includeParallelOptions
        )
        {
            ParallelOptions parallelOptions = null;

            if (includeParallelOptions)
            {
                parallelOptions = new ParallelOptions();
            }

            Assert.That(() => new PerformanceConfiguration(parallelOptions),
                Throws.Nothing);
        }

        [Test]
        public static void Constructor0_Succeeds()
        {
            Assert.That(() => new PerformanceConfiguration(),
                Throws.Nothing);
        }
    }
}
