using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Upmc.DevTools.VsParser.Entities;

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsParser
{
    /// <summary>
    ///     Parses GitVersion.yml files.
    /// </summary>
    public static class GitVersionParser
    {
        /// <summary>
        ///     Parse the contents of a GitVersion.yml file.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static GitVersion ParseGitVersionContent(string content)
        {
            var output = new GitVersion
            {
                AssemblyFileVersioningScheme = GetConfigString("Assembly-File-Versioning-Scheme", content),
                AssemblyInformationalFormat = GetConfigString("Assembly-Informational-Format", content),
                AssemblyVersioningScheme = GetConfigString("Assembly-Versioning-Scheme", content),
                BuildMetadataPadding = GetConfigInt("Build-metadata-padding", content),
                CommitDateFormat = GetConfigString("Commit-date-format", content),
                CommitMessageIncrementing = GetConfigBool("commit-message-incrementing", content),
                CommitsSinceVersionSourcePadding = GetConfigInt("commits-Since-Version-Source-Padding", content),
                ContinuousDeliveryFallbackTag = GetConfigString("continuous-delivery-fallback-tag", content),
                IgnoreCommitsBefore = GetConfigDateTime("commits-before", content),
                Increment = GetConfigString("increment", content),
                LegacySemVerPadding = GetConfigInt("Legacy-SemVer-padding", content),
                MajorVersionBumpMessage = GetConfigString("major-version-bump-message", content),
                MinorVersionBumpMessage = GetConfigString("minor-version-bump-message", content),
                PatchVersionBumpMessage = GetConfigString("patch-version-bump-message", content),
                NoBumpMessage = GetConfigString("no-bump-message", content),
                Mode = GetConfigEnum("mode", content),
                NextVersion = GetConfigString("next-version", content),
                TagPrefix = GetConfigString("tag-prefix", content)
            };
            output.IgnoreSha.AddRange(GetConfigShaCollection(content));

            return output;
        }

        private static string GetConfigString(string settingName, string content)
        {
            var searchRegex = new Regex(
                $@"^{settingName}\s*:\s*(.+)$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var output = GetValue(searchRegex, content);

            return output;
        }

        private static IEnumerable<string> GetConfigShaCollection(string content)
        {
            var output = new List<string>();

            /*
             sha:
                - e7bc24c0f34728a25c9187b8d0b041d935763e3a
                - 764e16321318f2fdb9cdeaa56d1156a1cba307d7
             */

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var line in content
                .Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries))
            {
                var searchRegex = new Regex(
                    @"^[ ]+-[ ]+(.+)\r?$",
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                var value = GetValue(searchRegex, line);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    output.Add(value);
                }
            }

            return output;
        }

        private static VersioningMode? GetConfigEnum(string settingName, string content)
        {
            VersioningMode? output = null;

            var searchRegex = new Regex(
                $"^{settingName}:(.+)$",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var value = GetValue(searchRegex, content);

            if (
                !string.IsNullOrWhiteSpace(value)
                &&
                Enum.TryParse<VersioningMode>(value, true, out var tempOutput))
            {
                output = tempOutput;
            }

            return output;
        }

        private static string GetValue(Regex searchRegex, string line)
        {
            var groupCollection = searchRegex.Match(line).Groups;
            var value = groupCollection.Count == 2
                ? groupCollection[1].Value
                : null;

            return value?.Trim();
        }

        private static int? GetConfigInt(string settingName, string content)
        {
            int? output = null;

            var searchRegex = new Regex(
                $"^{settingName}:(.+)$",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var value = GetValue(searchRegex, content);

            if (
                !string.IsNullOrWhiteSpace(value)
                &&
                int.TryParse(value, out var tempOutput))
            {
                output = tempOutput;
            }

            return output;
        }

        private static bool? GetConfigBool(string settingName, string content)
        {
            bool? output = null;

            var searchRegex = new Regex(
                $"^{settingName}:(.+)$",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            var value = GetValue(searchRegex, content);

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (value.Equals("Enabled", StringComparison.CurrentCultureIgnoreCase))
                {
                    output = true;
                }

                if (value.Equals("Disabled", StringComparison.CurrentCultureIgnoreCase))
                {
                    output = false;
                }
            }

            return output;
        }

        private static DateTime? GetConfigDateTime(string settingName, string content)
        {
            DateTime? output = null;

            var searchRegex = new Regex(
                $"^{settingName}:(.+)$",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            var value = GetValue(searchRegex, content);

            if (
                !string.IsNullOrWhiteSpace(value)
                &&
                DateTime.TryParse(value, out var tempOutput))
            {
                output = tempOutput;
            }

            return output;
        }
    }
}