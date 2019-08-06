using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using NLog;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Entities;

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsTs.SourceTools
{
    /// <inheritdoc />
    /// <summary>
    ///     A set of Git operations which run on the command-line
    /// </summary>
    [Obsolete("Use GitSourceTool instead.")]
    [ExcludeFromCodeCoverage]
    public class GitToolCommandLine : WaitAndRetryBase
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandLineTool">An instance of the command-line tool to be used.</param>
        /// <param name="serverUri">The Azure DevOps server's URL</param>
        /// <param name="workingDirectory">The working directory where the operations will take place</param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public GitToolCommandLine(
            ICommandLineTool commandLineTool,
            Uri serverUri,
            string workingDirectory,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(serverUri, nameof(serverUri));
            Validators.AssertIsNotNullOrWhitespace(workingDirectory, nameof(workingDirectory));

            _commandLineTool = commandLineTool;
            _serverUri = serverUri;
            _workingDirectory = workingDirectory;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        ///     Add (Stage) items in the current working directory.
        /// </summary>
        /// <param name="filePattern">Optional pattern.</param>
        public void Add(string filePattern = ".")
        {
            var process = _commandLineTool.ExecuteCommand("git", $"add {filePattern}", _workingDirectory);

            while (!process.HasExited)
            {
                Thread.Sleep(200);
            }

            if (process.ExitCode != 0)
            {
                throw new VsTsToolException(process.StandardOutput.ReadToEnd());
            }
        }

        /// <summary>
        ///     Branch in the local working directory.
        /// </summary>
        /// <param name="branchName"></param>
        public void Branch(string branchName)
        {
            Validators.AssertIsNotNullOrWhitespace(branchName, nameof(branchName));

            var process = _commandLineTool.ExecuteCommand("git", $"branch {branchName}", _workingDirectory);

            while (!process.HasExited)
            {
                Thread.Sleep(200);
            }

            if (process.ExitCode != 0)
            {
                throw new VsTsToolException(process.StandardOutput.ReadToEnd());
            }
        }

        /// <summary>
        ///     Check Out to the local working directory
        /// </summary>
        /// <param name="branchName">Optional Branch Name</param>
        public void CheckOut(string branchName = null)
        {
            var process = _commandLineTool.ExecuteCommand("git", $"checkout {branchName ?? ""}", _workingDirectory);

            while (!process.HasExited)
            {
                Thread.Sleep(200);
            }

            if (process.ExitCode != 0)
            {
                throw new VsTsToolException(process.StandardOutput.ReadToEnd());
            }
        }

        /// <summary>
        ///     Clone a Git repo to the working directory.
        /// </summary>
        public void Clone()
        {
            // This has to use the base path since clone will create a folder for the repo.
            var process = _commandLineTool.ExecuteCommand("git", $"clone {_serverUri}", _workingDirectory);

            while (!process.HasExited)
            {
                Thread.Sleep(200);
            }

            if (process.ExitCode != 0)
            {
                throw new VsTsToolException(process.StandardOutput.ReadToEnd());
            }
        }

        #endregion Public Methods

        #region Private Properties and Fields

        private readonly ICommandLineTool _commandLineTool;

        private readonly Uri _serverUri;

        private readonly string _workingDirectory;

        #endregion Private Properties and Fields
    }
}