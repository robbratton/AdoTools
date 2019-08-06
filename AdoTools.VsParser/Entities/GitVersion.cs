using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverQueried.Global

// ReSharper disable MissingXmlDoc
// ReSharper disable UnusedMember.Global

namespace AdoTools.VsParser.Entities
{
    /// <summary>
    ///     Represents the contents of a GitVersion.yml file.
    ///     See https://gitversion.readthedocs.io/en/latest/configuration/
    /// </summary>
    public class GitVersion
    {
        /// <summary>
        ///     Next Version Number
        /// </summary>
        public string NextVersion { get; set; }

        /// <summary>
        ///     Assembly Versioning Scheme
        /// </summary>
        public string AssemblyVersioningScheme { get; set; }

        /// <summary>
        ///     Assembly File Versioning Scheme
        /// </summary>
        public string AssemblyFileVersioningScheme { get; set; }

        /// <summary>
        ///     Assembly Informational Format
        /// </summary>
        public string AssemblyInformationalFormat { get; set; }

        /// <summary>
        ///     Versioning Mode
        /// </summary>
        public VersioningMode? Mode { get; set; }

        /// <summary>
        ///     Increment: Major, Minor, Patch, None, Inherit
        /// </summary>
        public string Increment { get; set; }

        /// <summary>
        ///     Continuous Delivery Fallback Tag
        /// </summary>
        public string ContinuousDeliveryFallbackTag { get; set; }

        /// <summary>
        ///     Tag Prefix
        /// </summary>
        public string TagPrefix { get; set; }

        /// <summary>
        ///     Major Version Bump Message
        /// </summary>
        public string MajorVersionBumpMessage { get; set; }

        /// <summary>
        ///     Minor Version Bump Message
        /// </summary>
        public string MinorVersionBumpMessage { get; set; }

        /// <summary>
        ///     Patch Version Bump Message
        /// </summary>
        public string PatchVersionBumpMessage { get; set; }

        /// <summary>
        ///     No Bump Message
        /// </summary>
        public string NoBumpMessage { get; set; }

        /// <summary>
        ///     Legacy SemVer Padding
        /// </summary>
        public int? LegacySemVerPadding { get; set; }

        /// <summary>
        ///     Build Metadata Padding
        /// </summary>
        public int? BuildMetadataPadding { get; set; }

        /// <summary>
        ///     Commits Since Version Source Padding
        /// </summary>
        public int? CommitsSinceVersionSourcePadding { get; set; }

        /// <summary>
        ///     Commit Message Incrementing
        /// </summary>
        /// <remarks>Enabled = true, Disabled = false</remarks>
        public bool? CommitMessageIncrementing { get; set; }

        /// <summary>
        ///     Commit Date Format
        /// </summary>
        public string CommitDateFormat { get; set; }

        /// <summary>
        ///     Ignore SHA
        /// </summary>
        public List<string> IgnoreSha { get; } = new List<string>();

        /// <summary>
        ///     Ignore Commits Before
        /// </summary>
        public DateTime? IgnoreCommitsBefore { get; set; }

        // ReSharper disable CommentTypo
        /*  Example Content
        next-version: 1.0
        assembly-versioning-scheme: MajorMinorPatch
        assembly-file-versioning-scheme: MajorMinorPatchTag
        assembly-informational-format: '{InformationalVersion}'
        mode: ContinuousDelivery
        increment: Inherit
        continuous-delivery-fallback-tag: ci
        tag-prefix: '[vV]'
        major-version-bump-message: '\+semver:\s?(breaking|major)'
        minor-version-bump-message: '\+semver:\s?(feature|minor)'
        patch-version-bump-message: '\+semver:\s?(fix|patch)'
        no-bump-message: '\+semver:\s?(none|skip)'
        legacy-semver-padding: 4
        build-metadata-padding: 4
        commits-since-version-source-padding: 4
        commit-message-incrementing: Enabled
        commit-date-format: 'yyyy-MM-dd'
        ignore:
            sha: []
            commits-before: yyyy-MM-ddTHH:mm:ss
         */
        // ReSharper restore CommentTypo
    }

    /// <summary>
    ///     Versioning Mode
    /// </summary>
    public enum VersioningMode
    {
        /// <summary>
        ///     Continuous Delivery
        /// </summary>
        ContinuousDelivery,

        /// <summary>
        ///     Continuous Integration
        /// </summary>
        ContinuousDeployment,

        /// <summary>
        ///     Mainline Development
        /// </summary>
        MainlineDevelopment
    }
}