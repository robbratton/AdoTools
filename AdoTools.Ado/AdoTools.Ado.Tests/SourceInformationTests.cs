using System;
using FluentAssertions;
using NUnit.Framework;
using Upmc.DevTools.VsTs.Entities;

namespace Upmc.DevTools.VsTs.Tests
{
    [TestFixture]
    public static class SourceInformationTests
    {
        private const string FileSourcePath = @"c:\git";

        private const string GitBranch = "develop";

        private const string GitRepository = "Upmc.DevTools.VsTs";

        private const string GitSourcePath = "/";

        private const string VcSourcePath = "$/Apollo/junk";

        private static SourceInformation GenerateSourceInformation(SourceType sourceType)
        {
            SourceInformation output = null;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (sourceType)
            {
                case SourceType.Filesystem:
                    output = new SourceInformation(sourceType, FileSourcePath, true);

                    break;
                case SourceType.TfsGit:
                    output = new SourceInformation(sourceType, GitSourcePath, true, GitRepository, GitBranch);

                    break;
                case SourceType.TfsVc:
                    output = new SourceInformation(sourceType, VcSourcePath, true);

                    break;
                case SourceType.None:
                    output = new SourceInformation();

                    break;
            }

            return output;
        }

        [Test]
        public static void AssertIsSourceType_ReturnsExpectedValue()
        {
            var input = GenerateSourceInformation(SourceType.Filesystem);

            Assert.Multiple(
                () =>
                {
                    Assert.That(
                        () =>
                            input.AssertIsSourceType(SourceType.TfsGit),
                        Throws.ArgumentException.With.Message.Contains("Source type is not"));

                    Assert.That(
                        () =>
                            input.AssertIsSourceType(SourceType.Filesystem),
                        Throws.Nothing);
                });
        }

        [Test]
        public static void AssertIsValid_ReturnsExpectedValue([Values] bool isValid)
        {
            var input = GenerateSourceInformation(SourceType.Filesystem);

            if (!isValid)
            {
                input.SourcePath = null;
                Assert.That(
                    () =>
                        input.AssertIsValid(),
                    Throws.ArgumentException.With.Message.Contains("SourcePath must be provided"));
            }
            else
            {
                Assert.That(
                    () =>
                        input.AssertIsValid(),
                    Throws.Nothing);
            }
        }

        [Test]
        public static void ConstructorCopy_Succeeds()
        {
            var input = new SourceInformation(SourceType.TfsGit, "path", true, "repository", "branch");

            var result = new SourceInformation(input);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsEmpty, Is.False);
            Assert.That(result.IsValid, Is.True);

            result.Should().BeEquivalentTo(input);
        }

        [Test]
        public static void ConstructorWithNoParams_Succeeds()
        {
            var result = new SourceInformation();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsEmpty, Is.True);
            Assert.That(result.IsValid, Is.True);

            Assert.That(result.SourceType, Is.EqualTo(SourceType.None));
            Assert.That(result.GitBranch, Is.Null);
            Assert.That(result.GitRepositoryName, Is.Null);
            Assert.That(result.SourcePath, Is.Null);
            Assert.That(result.PathSeparator, Is.Null);
        }

        [Test]
        public static void ConstructorWithType_Succeeds([Values] SourceType sourceType)
        {
            var result = GenerateSourceInformation(sourceType);

            Assert.That(result, Is.Not.Null);

            Assert.That(result.SourceType, Is.EqualTo(sourceType));
            Assert.That(result.IsValid, Is.True);

            Assert.That(
                result.PathSeparator,
                sourceType == SourceType.None
                    ? Is.Null
                    : Is.Not.Null);

            switch (sourceType)
            {
                case SourceType.Filesystem:
                    Assert.That(result.GitBranch, Is.Null);
                    Assert.That(result.GitRepositoryName, Is.Null);
                    Assert.That(result.SourcePath, Is.EqualTo(FileSourcePath));
                    Assert.That(result.PathSeparator, Is.Not.Null);

                    break;
                case SourceType.None:
                    Assert.That(result.GitBranch, Is.Null);
                    Assert.That(result.GitRepositoryName, Is.Null);
                    Assert.That(result.SourcePath, Is.Null);
                    Assert.That(result.PathSeparator, Is.Null);

                    break;
                case SourceType.TfsGit:
                    Assert.That(result.GitBranch, Is.EqualTo(GitBranch));
                    Assert.That(result.GitRepositoryName, Is.EqualTo(GitRepository));
                    Assert.That(result.SourcePath, Is.EqualTo(GitSourcePath));
                    Assert.That(result.PathSeparator, Is.Not.Null);

                    break;
                case SourceType.TfsVc:
                    Assert.That(result.GitBranch, Is.Null);
                    Assert.That(result.GitRepositoryName, Is.Null);
                    Assert.That(result.SourcePath, Is.EqualTo(VcSourcePath));
                    Assert.That(result.PathSeparator, Is.Not.Null);

                    break;
                default:
                    Assert.Inconclusive($"{nameof(sourceType)} of {sourceType} is not supported");

                    break;
            }
        }

        [Test]
        public static void ToString_ReturnsExpectedValue([Values] SourceType sourceType)
        {
            var input = GenerateSourceInformation(sourceType);

            var result = input.ToString();

            if (input.SourceType == SourceType.None)
            {
                StringAssert.Contains("Type", result);
                StringAssert.DoesNotContain("Path", result);
                StringAssert.DoesNotContain("Repo", result);
                StringAssert.DoesNotContain("Branch", result);
            }
            else
            {
                StringAssert.Contains("Type", result);
                StringAssert.Contains("Path", result);

                if (input.SourceType == SourceType.TfsGit)
                {
                    StringAssert.Contains("Repo", result);
                    StringAssert.Contains("Branch", result);
                }
                else
                {
                    StringAssert.DoesNotContain("Repo", result);
                    StringAssert.DoesNotContain("Branch", result);
                }
            }
        }

        [Test]
        public static void ValidationMessage_ReturnsExpectedValue([Values(0, 1, 2, 3)] int testCase)
        {
            var input = GenerateSourceInformation(SourceType.TfsGit);

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (testCase)
            {
                case 0:
                    Assert.That(input.ValidationMessage, Is.EqualTo(string.Empty));

                    break;
                case 1:
                    input.SourcePath = null;
                    StringAssert.Contains("SourcePath", input.ValidationMessage);

                    break;
                case 2:
                    input.GitBranch = null;
                    input.GitRepositoryName = null;
                    StringAssert.Contains("Git", input.ValidationMessage);

                    break;
                case 3:
                    input.SourceType = SourceType.None;
                    StringAssert.Contains("is not supported", input.ValidationMessage);

                    break;
            }
        }
    }
}