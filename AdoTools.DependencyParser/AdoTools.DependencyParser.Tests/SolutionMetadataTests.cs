using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Upmc.DevTools.Dependency.Parser.Entities;
using Upmc.DevTools.VsParser.Entities.Project;

namespace Upmc.DevTools.Dependency.Parser.Tests
{
    [TestFixture]
    public class SolutionMetadataTests : TestBase
    {
        [Test]
        public static void Constructor1_Succeeds()
        {
            var result = new SolutionMetadata();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public static void Constructor2_Succeeds(
            [Values(null, "", "  ", "name")] string name,
            [Values] bool sourceInformationIsNull,
            [Values] bool solutionIsNull
        )
        {
            // Arrange
            var result = Helpers.CreateSolutionMetadata(
                name,
                sourceInformationIsNull,
                solutionIsNull,
                out var sourceInformation,
                out var solution);

            // Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(result, Is.Not.Null);
                    Assert.That(result.Name, Is.EqualTo(name));

                    result.SourceInformation.Should().BeEquivalentTo(sourceInformation);

                    if (solutionIsNull)
                    {
                        Assert.That(result.Projects, Is.Empty);
                        Assert.That(result.FormatVersion, Is.Null);
                        Assert.That(result.MinimumVisualStudioVersion, Is.Null);
                        Assert.That(result.VisualStudioVersion, Is.Null);
                    }
                    else
                    {
                        result.Should().BeEquivalentTo(
                            solution,
                            opt => opt
                                .ExcludingMissingMembers()
                                .Excluding(x => x.Projects)
                        );

                        result.Projects.Should().BeEquivalentTo(
                            solution.Projects,
                            opt => opt
                                .ExcludingMissingMembers()
                        );
                    }
                });
        }

        [Test]
        public static void MapFromSolution_ReturnsExpectedResult()
        {
            var solution = Helpers.CreateTestSolution();

            var result = SolutionMetadata.MapFromSolution(solution);

            result.Should().BeEquivalentTo(
                solution,
                options => options
                    .ExcludingMissingMembers()
                    .Excluding(x => x.Projects)
            );

            var stuff = result.ProjectMetadatas.Cast<Project>().ToList();

            stuff.Should().AllBeEquivalentTo(
                solution.Projects,
                options => options.ExcludingMissingMembers());
        }

        [Test]
        public static void Update_ReturnsExpectedResult()
        {
            var solutionMetadata1 = Helpers.CreateSolutionMetadata("name1", false, false, out _, out _);
            var solutionMetadata2 = Helpers.CreateSolutionMetadata("name2", false, false, out _, out _);

            solutionMetadata1.Update(solutionMetadata2);

            // todo Assert that changes were made.
            Assert.That(solutionMetadata1.Name, Is.EqualTo(solutionMetadata2.Name));
        }

        [Test]
        public static void UpdateFromSolution_ReturnsExpectedResult()
        {
            var solutionMetadata1 = Helpers.CreateSolutionMetadata("name", false, false, out _, out _);

            var solution2 = Helpers.CreateTestSolution();

            solutionMetadata1.UpdateFromSolution(solution2);

            // todo Assert that changes were made.
            Assert.That(
                solutionMetadata1.Projects.FirstOrDefault()?.Id,
                Is.EqualTo(solution2.Projects.FirstOrDefault()?.Id));
        }
    }
}