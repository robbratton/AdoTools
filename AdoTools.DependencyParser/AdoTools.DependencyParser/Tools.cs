using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NLog;
using Upmc.DevTools.Common;
using Upmc.DevTools.Dependency.Parser.Entities;
using Upmc.DevTools.VsTs.Entities;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.Dependency.Parser
{
    /// <summary>
    ///     General Tools for parsing
    /// </summary>
    public static class Tools
    {
        private static readonly Regex[] EnvironmentRegularExpressions;

        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static Tools()
        {
            Logger.Trace("Entering");

            EnvironmentRegularExpressions = new List<Regex>
            {
                new Regex(@"^appsettings\.(.+)\.json$", RegexOptions.IgnoreCase | RegexOptions.Compiled),
                new Regex(@"^dataConfiguration\.(.+)\.config$", RegexOptions.IgnoreCase | RegexOptions.Compiled),
                new Regex(@"^web\.(.+)\.config$", RegexOptions.IgnoreCase | RegexOptions.Compiled),
                new Regex(@"^app\.(.+)\.config$", RegexOptions.IgnoreCase | RegexOptions.Compiled)
            }.ToArray();
        }

        /// <summary>
        ///     Fix path endings by making sure directories end with a path separator
        /// </summary>
        /// <param name="sourceInformation"></param>
        public static void FixPathEnding(SourceInformation sourceInformation)
        {
            Logger.Trace("Entering");

            sourceInformation.SourcePath = FixPathEnding(
                sourceInformation.SourcePath,
                sourceInformation.PathSeparator,
                sourceInformation.IsDirectory);
        }

        /// <summary>
        ///     Fix path endings by making sure directories end with a path separator
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pathSeparator"></param>
        /// <param name="isDirectory"></param>
        /// <returns></returns>
        public static string FixPathEnding(string input, string pathSeparator, bool isDirectory)
        {
            Logger.Trace("Entering");

            var output = input;

            if (isDirectory && !output.EndsWith(pathSeparator))
            {
                // Add missing trailing separator to directory, if necessary.
                output += pathSeparator;
            }

            return output;
        }

        /// <summary>
        ///     Fixes path separators
        /// </summary>
        /// <param name="sourceInformation"></param>
        public static void FixPathSeparators(SourceInformation sourceInformation)
        {
            Logger.Trace("Entering");

            sourceInformation.SourcePath = FixPathSeparators(
                sourceInformation.SourcePath,
                sourceInformation.PathSeparator);
        }

        /// <summary>
        ///     Fixes path separators.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pathSeparator"></param>
        /// <returns></returns>
        public static string FixPathSeparators(string input, string pathSeparator)
        {
            Logger.Trace("Entering");

            var output = input;

            // Replace path separators if necessary.
            switch (pathSeparator)
            {
                case "/":
                    output = output.Replace("\\", "/");

                    break;

                case "\\":
                    output = output.Replace("/", "\\");

                    break;

                default:

                    throw new ArgumentException(
                        $"Value '{pathSeparator}' is not supported.",
                        nameof(pathSeparator));
            }

            return output;
        }

        /// <summary>
        ///     Gets Information about the Child Directory
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <param name="childDirectoryName"></param>
        /// <returns></returns>
        public static SourceInformation GetChildDirectoryInformation(
            SourceInformation sourceInformation,
            string childDirectoryName)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();
            Validators.AssertIsNotNullOrWhitespace(childDirectoryName, nameof(childDirectoryName));

            Logger.Trace("Entering");

            var directoryName = Path.GetDirectoryName(sourceInformation.SourcePath);

            if (directoryName == null)
            {
                throw new ArgumentException(
                    $"Cannot find the directory name for the {nameof(sourceInformation.SourcePath)} in {nameof(sourceInformation)}");
            }

            var newSourceInformation = new SourceInformation(sourceInformation)
            {
                SourcePath = Path.Combine(directoryName, childDirectoryName),
                IsDirectory = true
            };

            FixPathSeparators(newSourceInformation);
            FixPathEnding(newSourceInformation);

            return newSourceInformation;
        }

        /// <summary>
        ///     Gets Information about the Child File
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SourceInformation GetChildFileInformation(SourceInformation sourceInformation, string filename)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();

            Logger.Trace("Entering");

            var directoryName = Path.GetDirectoryName(sourceInformation.SourcePath);

            if (directoryName == null)
            {
                throw new ArgumentException(
                    $"Cannot find the directory name for the {nameof(sourceInformation.SourcePath)} in {nameof(sourceInformation)}");
            }

            var newSourceInformation = new SourceInformation(sourceInformation)
            {
                SourcePath = Path.Combine(directoryName, filename),
                IsDirectory = false
            };

            FixPathSeparators(newSourceInformation);
            FixPathEnding(newSourceInformation);

            return newSourceInformation;
        }

        /// <summary>
        ///     Gets Directory Information
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <returns></returns>
        public static SourceInformation GetDirectoryInformation(SourceInformation sourceInformation)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();

            Logger.Trace("Entering");

            var newSourceInformation =
                new SourceInformation(sourceInformation)
                {
                    SourcePath = Path.GetDirectoryName(sourceInformation.SourcePath),
                    IsDirectory = true
                };

            FixPathSeparators(newSourceInformation);
            FixPathEnding(newSourceInformation);

            return newSourceInformation;
        }

        /// <summary>
        ///     Gets Directory Information given a Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DeployEnvironment GetEnvironmentFromSourcePath(string path)
        {
            Logger.Trace("Entering");

            var output = DeployEnvironment.None;

            if (!string.IsNullOrWhiteSpace(path))
            {
                var fileName = Path.GetFileName(path);

                foreach (var regularExpression in EnvironmentRegularExpressions)
                {
                    var match = regularExpression.Match(fileName);

                    if (match.Success && match.Groups.Count == 2)
                    {
                        var envString = match.Groups[1].Value;
                        output = DeployEnvironmentHelper.MapEnvironment(envString);

                        break;
                    }
                }
            }

            return output;
        }

        /// <summary>
        ///     Gets Parent Directory Information for this SourceInformation
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <returns></returns>
        public static SourceInformation GetParentDirectoryInformation(SourceInformation sourceInformation)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();

            Logger.Trace("Entering");

            var newSourceInformation = new SourceInformation(sourceInformation)
            {
                SourcePath = Path.GetDirectoryName(Path.GetDirectoryName(sourceInformation.SourcePath)),
                IsDirectory = true
            };

            FixPathSeparators(newSourceInformation);
            FixPathEnding(newSourceInformation);

            return newSourceInformation;
        }
    }
}