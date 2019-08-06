using System;
using System.Reflection;
using NLog;
using NUnit.Framework;

namespace Upmc.DevTools.Dependency.Parser.Tests
{
    public class TestBase
    {
        // ReSharper disable once InconsistentNaming
        protected static Logger _logger;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _logger = LogManager.GetCurrentClassLogger();

            _logger.Info($"Starting {GetType().FullName}");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            var entryAssembly = Assembly.GetExecutingAssembly();
            var _ = entryAssembly.GetName();
            _logger.Info($"Stopping {GetType().FullName}");
        }
    }
}