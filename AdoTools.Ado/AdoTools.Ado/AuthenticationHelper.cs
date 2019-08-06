using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NLog;
using Upmc.DevTools.Common;

// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

namespace Upmc.DevTools.VsTs
{
    /// <summary>
    ///     Helper Methods
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        ///     The Default PAth for the File Containing the Personal Access Token
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string DefaultTokenFilePath = @"%APPDATA%\vsts_Key.txt";

        /// <summary>
        ///     The Default Username to Use when Authenticating
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string DefaultUsername = "%USERNAME%@upmc.edu";

        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Tries to get the Personal Access Token for Azure DevOps from the users %APPDATA%\vsts_key.txt file.
        /// </summary>
        /// <param name="args">The arguments from the command line</param>
        /// <param name="path">The path to a text file containing the PersonalAccessToken, can include environment variables.</param>
        /// <returns>Azure DevOps Personal Access Token if found, otherwise throws <see cref="InvalidOperationException" />.</returns>
        public static string GetPersonalAccessToken(IEnumerable<string> args = null, string path = DefaultTokenFilePath)
        {
            Logger.Trace("Entering");

            string pat;

            var expandedPath = Environment.ExpandEnvironmentVariables(path);

            if (File.Exists(expandedPath))
            {
                pat = File.ReadAllText(expandedPath);
            }
            // ReSharper disable once PossibleMultipleEnumeration
            else if (args != null && args.Any())
            {
                // ReSharper disable once PossibleMultipleEnumeration
                pat = args.First();
            }
            else
            {
                throw new InvalidOperationException(
                    $"Personal Access Token must be provided as an argument or in the {expandedPath} file.");
            }

            return pat;
        }

        /// <summary>
        ///     Gets the username from the environment's USERNAME variable.
        /// </summary>
        /// <param name="username">The literal username to be used or a string with environment variables.</param>
        /// <returns>Domain username with domain suffix if found, otherwise throws <see cref="InvalidOperationException" />.</returns>
        public static string GetUsername(string username = DefaultUsername)
        {
            Validators.AssertIsNotNullOrWhitespace(username, username);

            Logger.Trace("Entering");

            var expandedUsername = Environment.ExpandEnvironmentVariables(username);

            if (string.IsNullOrWhiteSpace(expandedUsername))
            {
                throw new InvalidOperationException($"{username} was not found.");
            }

            return expandedUsername;
        }
    }
}