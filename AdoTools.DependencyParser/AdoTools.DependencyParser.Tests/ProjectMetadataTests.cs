using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Upmc.DevTools.Dependency.Parser.Entities;
using Upmc.DevTools.VsParser.Entities.ConfigFile.Components;
using Upmc.DevTools.VsParser.Entities.DataConfigurationFile.Components;
using Upmc.DevTools.VsParser.Entities.Project;
using Upmc.DevTools.VsParser.Entities.Project.Components;
using Upmc.DevTools.VsTs.Entities;

namespace Upmc.DevTools.Dependency.Parser.Tests
{
    [TestFixture]
    public class ProjectMetadataTests : TestBase
    {
        [Test]
        public static void Constructor1_Succeeds()
        {
            var result = new ProjectMetadata();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public static void Constructor2_Succeeds(
            [Values] bool sourceInformationIsNull,
            [Values] bool projectIsNull
        )
        {
            // Arrange
            SourceInformation sourceInformation = null;

            if (!sourceInformationIsNull)
            {
                sourceInformation = Helpers.CreateTestSourceInformation();
            }

            Project project = null;

            if (!projectIsNull)
            {
                project = Helpers.CreateTestProject();
            }

            // Act
            var result = new ProjectMetadata(sourceInformation, project);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(result, Is.Not.Null);

                    result.SourceInformation.Should().BeEquivalentTo(sourceInformation);

                    if (projectIsNull)
                    {
                        Assert.That(result.AssemblyInformation, Is.Empty);
                        Assert.That(result.AssemblyReferences, Is.Empty);
                        Assert.That(result.Configurations, Is.Empty);
                        Assert.That(result.Frameworks, Is.Empty);
                        Assert.That(result.Id, Is.Null);
                        Assert.That(result.NameInSolution, Is.Null);
                        Assert.That(result.PackageReferences, Is.Empty);
                        Assert.That(result.PathRelativeToSolution, Is.Null);
                        Assert.That(result.ProjectFileFormat, Is.Null);
                        Assert.That(
                            result.ProjectOutputType,
                            Is.EqualTo(ProjectOutputType.Library)); // This has a default value.
                        Assert.That(result.ProjectReferences, Is.Empty);
                        Assert.That(result.Sdk, Is.Null);
                        Assert.That(result.TypeIds, Is.Empty);
                    }
                    else
                    {
                        result.Should().BeEquivalentTo(
                            project,
                            opt => opt
                                .ExcludingMissingMembers()

                                // Exclude Project fields which come from the Solution.
                                .Excluding(y => y.NameInSolution)
                                .Excluding(y => y.TypeIds)
                                .Excluding(y => y.Id)
                                .Excluding(y => y.PathRelativeToSolution)
                        );
                    }
                });
        }

        [Test]
        public static void MapFromProject_ReturnsExpectedResult()
        {
            var project = Helpers.CreateTestProject();

            var pm = ProjectMetadata.MapFromProject(project);

            Assert.That(pm, Is.Not.Null);

            pm.Should().BeEquivalentTo(
                project,
                options => options.ExcludingMissingMembers());
        }

        [Test]
        public void Update_ReturnsExpectedResult()
        {
            // Original Item
            var sourceInformation1 = new SourceInformation(SourceType.Filesystem, @"c:\git", true);
            var project1 = Helpers.CreateTestProject();
            var pm1 =
                new ProjectMetadata(sourceInformation1, project1)
                {
                    AppSettings = new List<ApplicationSetting>(),
                    ConnectionStrings = new List<ConnectionStringSetting>
                        {new ConnectionStringSetting {Name = "test1"}},
                    DatabaseInstances = new List<DatabaseInstance>(),
                    DatabaseTypes = new List<DatabaseType>()
                };

            // pm1 will be updated with information from pm2
            var sourceInformation2 = new SourceInformation(SourceType.Filesystem, @"c:\git\dir2", true);
            var project2 = Helpers.CreateTestProject();
            // ReSharper disable once StringLiteralTypo
            project2.Frameworks = new List<string> {"netcoreapp1.0"};
            var pm2 = new ProjectMetadata(sourceInformation2, project2)
            {
                AppSettings = new List<ApplicationSetting>
                    {new ApplicationSetting("name", "value")},
                ConnectionStrings = new List<ConnectionStringSetting>(),
                DatabaseInstances = new List<DatabaseInstance>(),
                DatabaseTypes = new List<DatabaseType>()
            };

            pm1.Update(pm2);

            Assert.That(pm1, Is.Not.Null);

            pm1.Should().BeEquivalentTo(pm2);
        }
    }
}