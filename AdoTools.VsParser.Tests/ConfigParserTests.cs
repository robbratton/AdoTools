using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using AdoTools.VsParser.Entities;
using AdoTools.VsParser.Entities.Project.Components;

// ReSharper disable UnusedVariable

namespace AdoTools.VsParser.Tests
{
    [TestFixture]
    public class ConfigParserTests
    {
        [TestCase(@"directory\app.QA.Config", DeployEnvironment.Qa)]
        public static void GetEnvironmentFromPath_ReturnsExpectedResult(string path,
            DeployEnvironment expectedEnvironment)
        {
            var result = ConfigParser.GetEnvironmentFromPath(path);

            Assert.That(result, Is.EqualTo(expectedEnvironment));
        }

        [TestCase("same.txt")]
        [TestCase("other.csproj")]
        [TestCase("junk.sln")]
        public static void GetEnvironmentFromPath_Throws(string path)
        {
            Assert.That(() => ConfigParser.GetEnvironmentFromPath(path), Throws.ArgumentException);
        }

        [Test]
        public static void ParseAppConfigFileContent_ReturnsExpectedValues()
        {
            var content = Helpers.ReadEmbeddedResourceFile("app.config.txt");

            var result = ConfigParser.ParseAppConfigFileContent(content, "app.QA.config");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ApplicationSettings.Count, Is.EqualTo(4));

            // todo Do more asserts here.
        }

        [Test]
        public static void ParseAppConfigFileContent_Throws([Values(null, "", "  ")] string input)
        {
            Assert.Throws<ArgumentException>(() => ConfigParser.ParseAppConfigFileContent(input, "app.QA.config"));
        }

        [Test]
        public static void ParseAppSettingsJsonFileContent_ReturnsExpectedValues()
        {
            var content = Helpers.ReadEmbeddedResourceFile("AppSettings.tok.json.txt");

            var result = ConfigParser.ParseAppSettingsJsonFileContent(content);

            Assert.That(result, Is.Not.Null);

            Assert.Multiple(() =>
                {
                    Assert.That(result.AppSettings?.Count, Is.EqualTo(12), nameof(result.AppSettings) + " count");
                    Assert.That(result.AppSettings?["EnableSwagger"], Is.EqualTo("%{ENABLE_SWAGGER}%"),
                        nameof(result.AppSettings) + " - EnableSwagger");

                    Assert.That(result.ConnectionStrings, Is.Not.Null, nameof(result.ConnectionStrings));
                    Assert.That(result.ConnectionStrings?.Count, Is.EqualTo(4),
                        nameof(result.ConnectionStrings) + " count");
                    Assert.That(result.ConnectionStrings?["RedisConnection"], Is.EqualTo("%{REDIS_CONNECTION}%"),
                        nameof(result.ConnectionStrings) + " - RedisConnection");

                    Assert.That(result.CoreMonitoringSettingsConfiguration?.Count, Is.EqualTo(2),
                        nameof(result.CoreMonitoringSettingsConfiguration) + " count");

                    Assert.That(result.CoreServiceBusConfiguration?.Count, Is.EqualTo(9),
                        nameof(result.CoreServiceBusConfiguration) + " count");

                    Assert.That(result.CorePersistenceSettingsConfiguration?.Count, Is.EqualTo(1),
                        nameof(result.CorePersistenceSettingsConfiguration) + " count");
                    Assert.That(result.CorePersistenceSettingsConfiguration?["DbConnectionName"], Is.Not.Null,
                        nameof(result.CorePersistenceSettingsConfiguration) + " - DbConnectionName");

                    Assert.That(result.OAuth2?.Count, Is.EqualTo(2), nameof(result.OAuth2) + " count");
                    Assert.That(result.OAuth2?["Authority"], Is.EqualTo("%{AZURE_AD_AUTHORITY}%"),
                        nameof(result.OAuth2) + " - Authority");
                    Assert.That(result.OAuth2?["ApplicationId"], Is.EqualTo("%{AZURE_AD_APP_ID}%"),
                        nameof(result.OAuth2) + " - ApplicationId");
                }
            );

            // todo Do more asserts here?
        }

        [Test]
        public static void ParseAppSettingsJsonFileContent_Throws([Values(null, "", "  ")] string input)
        {
            Assert.Throws<ArgumentException>(() => ConfigParser.ParseAppSettingsJsonFileContent(input));
        }

        [Test]
        public static void ParseDataConfigFileContent_ReturnsExpectedValues()
        {
            // ReSharper disable once StringLiteralTypo
            var content = Helpers.ReadEmbeddedResourceFile("dataconfiguration.config.txt");

            var result = ConfigParser.ParseDataConfigurationFileContent(content, "dataConfiguration.QA.config");

            Assert.Multiple(
                () =>
                {
                    Assert.That(result.DatabaseTypes, Is.Not.Null);
                    Assert.That(result.DatabaseTypes.Count, Is.EqualTo(1));

                    Assert.That(result.DatabaseInstances, Is.Not.Null);
                    Assert.That(result.DatabaseInstances.Count, Is.EqualTo(10));

                    Assert.That(result.ConnectionStrings, Is.Not.Null);
                    Assert.That(result.ConnectionStrings.Count, Is.EqualTo(11));

                    // todo Do more asserts here.
                });
        }

        [Test]
        public static void ParseDataConfigFileContent_Throws([Values(null, "", "  ")] string input)
        {
            Assert.Throws<ArgumentException>(
                () => ConfigParser.ParseDataConfigurationFileContent(input, "dataConfiguration.QA.config"));
        }

        [Test]
        public static void ParseWebConfigFileContent_ReturnsExpectedValues()
        {
            var content = Helpers.ReadEmbeddedResourceFile("web.config.txt");

            var result = ConfigParser.ParseWebConfigFileContent(content, "web.QA.config");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AppSettings.Count, Is.EqualTo(1));

            // todo Do more asserts here.
        }

        [Test]
        public static void ParseWebConfigFileContent_Throws([Values(null, "", "  ")] string input)
        {
            Assert.Throws<ArgumentException>(() => ConfigParser.ParseWebConfigFileContent(input, "web.QA.config"));
        }

        [Test]
        public void SetDeployEnvironment_ReturnsExpectedResult()
        {
            IEnumerable<ISettingWithEnvironment> settings = new[]
            {
                new AssemblyReference {Environment = DeployEnvironment.Production},
                new AssemblyReference {Environment = DeployEnvironment.Training}
            };

            const DeployEnvironment expectedEnvironment = DeployEnvironment.Lab;

            ConfigParser.SetDeployEnvironment(settings, expectedEnvironment);

            Assert.That(settings.All(s => s.Environment == expectedEnvironment));
        }
    }
}