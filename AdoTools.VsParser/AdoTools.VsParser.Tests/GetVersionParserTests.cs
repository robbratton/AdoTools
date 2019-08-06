using System;
using NUnit.Framework;
using Upmc.DevTools.VsParser.Entities;

// ReSharper disable StringLiteralTypo

namespace Upmc.DevTools.VsParser.Tests
{
    [TestFixture]
    public static class GetVersionParserTests
    {
        [TestCase(1, "1.2.3")]
        [TestCase(2, null)]
        [TestCase(3, "1.2.3")]
        [TestCase(4, "1.2.3")]
        [TestCase(5, "1.2.3")]
        public static void ParseGitVersionFile_NextVersion_ReturnsExpectedOutput(int fileNumber, string expectedResult)
        {
            var input = Helpers.ReadEmbeddedResourceFile($@"GitVersion{fileNumber}.yml.txt");

            var result = GitVersionParser.ParseGitVersionContent(input);

            Assert.That(result.NextVersion, Is.EqualTo(expectedResult));
        }

        [Test]
        public static void ParseGitVersionContent_ReturnsExpectedOutput()
        {
            var input = Helpers.ReadEmbeddedResourceFile(@"GitVersionFull.yml.txt");

            var result = GitVersionParser.ParseGitVersionContent(input);


        Assert.Multiple(
                () =>
                {
                    Assert.That(result.NextVersion, Is.EqualTo("1.0"), "NextVersion");
                    Assert.That(
                        result.AssemblyVersioningScheme,
                        Is.EqualTo("MajorMinorPatch"),
                        "AssemblyVersioningScheme");
                    Assert.That(
                        result.AssemblyFileVersioningScheme,
                        Is.EqualTo("MajorMinorPatchTag"),
                        "AssemblyFileVersioningScheme");
                    Assert.That(
                        result.AssemblyInformationalFormat,
                        Is.EqualTo("'{InformationalVersion}'"),
                        "AssemblyInformationalFormat");
                    Assert.That(result.Mode, Is.EqualTo(VersioningMode.ContinuousDelivery), "Mode");
                    Assert.That(result.Increment, Is.EqualTo("Inherit"), "Increment");
                    Assert.That(
                        result.ContinuousDeliveryFallbackTag,
                        Is.EqualTo("ci"),
                        "ContinuousDeliveryFallbackTag");
                    Assert.That(result.TagPrefix, Is.EqualTo("'[vV]'"), "TagPrefix");
                    Assert.That(
                        result.MajorVersionBumpMessage,
                        Is.EqualTo(@"'\+semver:\s?(breaking|major)'"),
                        "MajorVersionBumpMessage");
                    Assert.That(
                        result.MinorVersionBumpMessage,
                        Is.EqualTo(@"'\+semver:\s?(feature|minor)'"),
                        "MinorVersionBumpMessage");
                    Assert.That(
                        result.PatchVersionBumpMessage,
                        Is.EqualTo(@"'\+semver:\s?(fix|patch)'"),
                        "PatchVersionBumpMessage");
                    Assert.That(result.NoBumpMessage, Is.EqualTo(@"'\+semver:\s?(none|skip)'"), "NoBumpMessage");
                    Assert.That(result.LegacySemVerPadding, Is.EqualTo(4), "LegacySemVerPadding");
                    Assert.That(result.BuildMetadataPadding, Is.EqualTo(4), "BuildMetadataPadding");
                    Assert.That(
                        result.CommitsSinceVersionSourcePadding,
                        Is.EqualTo(4),
                        "CommitsSinceVersionSourcePadding");
                    Assert.That(result.CommitMessageIncrementing, Is.True, "CommitMessageIncrementing");
                    Assert.That(result.CommitDateFormat, Is.EqualTo("'yyyy-MM-dd'"), "CommitDateFormat");
                    Assert.That(result.IgnoreSha.Count, Is.EqualTo(2), "IgnoreSha.Count");
                    Assert.That(
                        result.IgnoreCommitsBefore,
                        Is.EqualTo(new DateTime(2018, 1, 1, 1, 2, 3)),
                        "IgnoreCommitsBefore");
                }
            );
        }
    }
}