using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NLog;
using Upmc.DevTools.Common;

// ReSharper disable UnusedMember.Global

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

namespace Upmc.DevTools.VsTs.Entities
{
    /// <summary>
    ///     Source location information
    /// </summary>
    [Serializable]
    public class SourceInformation
    {
        /// <summary>
        ///     NLog Logger Instance
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [NonSerialized] private static Logger _logger;

        /// <summary>
        ///     Git Branch name
        /// </summary>
        public string GitBranch { get; set; }

        /// <summary>
        ///     Git Repository name
        /// </summary>
        public string GitRepositoryName { get; set; }

        /// <summary>
        ///     Indicates Whether this Instance is Valid
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        ///     Indicates if this class is empty.
        /// </summary>
        public bool IsEmpty => SourceType == SourceType.None
                               && SourcePath == null
                               && GitBranch == null
                               && GitRepositoryName == null;

        /// <summary>
        ///     Indicates Whether this Instance is Valid
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsValid => ValidationMessage.Length == 0;

        /// <summary>
        ///     Returns the appropriate path separator character based upon the SourceType.
        /// </summary>
        public string PathSeparator
        {
            get
            {
                string output;

                switch (SourceType)
                {
                    case SourceType.TfsVc:
                        output = "/";

                        break;
                    case SourceType.TfsGit:
                        output = "/";

                        break;
                    case SourceType.Filesystem:
                        output = "\\";

                        break;
                    case SourceType.None:
                        output = null;

                        break;
                    default:

                        throw new ArgumentOutOfRangeException();
                }

                return output;
            }
        }

        /// <summary>
        ///     The VC or Git source repository path like $/Apollo/xxx for VC
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        ///     The type of the source path
        /// </summary>
        public SourceType SourceType { get; set; }

        /// <summary>
        ///     Returns any validation messages
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string ValidationMessage
        {
            get
            {
                _logger.Trace("Entering");

                var output = new List<string>();

                if (SourceType != SourceType.None && string.IsNullOrWhiteSpace(SourcePath))
                {
                    output.Add($"{nameof(SourcePath)} must be provided.");
                }

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (SourceType == SourceType.None && !IsEmpty)
                {
                    output.Add(
                        $"{nameof(SourceType)} of {SourceType.None} is not supported except for empty instances.");
                }

                if (SourceType == SourceType.TfsGit && string.IsNullOrWhiteSpace(GitRepositoryName))
                {
                    output.Add($" {nameof(GitRepositoryName)} must be provided.");
                }

                if (SourceType != SourceType.TfsGit
                    && (!string.IsNullOrWhiteSpace(GitRepositoryName) || !string.IsNullOrWhiteSpace(GitBranch)))
                {
                    output.Add($" {nameof(GitRepositoryName)} and {nameof(GitBranch)}must not be provided.");
                }

                return string.Join(", ", output);
            }
        }

        /// <summary>
        ///     Throws an argumentException if this source type is not the required type.
        /// </summary>
        /// <param name="sourceControlType"></param>
        public void AssertIsSourceType(SourceType sourceControlType)
        {
            _logger.Trace("Entering");

            if (SourceType != sourceControlType)
            {
                throw new ArgumentException($"Source type is not {sourceControlType}.");
            }
        }

        /// <summary>
        ///     Throws an ArgumentException if this source information is not valid.
        /// </summary>
        public void AssertIsValid()
        {
            _logger.Trace("Entering");

            if (!IsValid)
            {
                throw new ArgumentException(ValidationMessage);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            _logger.Trace("Entering");

            string output;

            switch (SourceType)
            {
                case SourceType.TfsGit:
                    output = $"Type={SourceType} Repo={GitRepositoryName} Branch={GitBranch} Path={SourcePath}";

                    break;
                case SourceType.TfsVc:
                case SourceType.Filesystem:
                    output = $"Type={SourceType} Path={SourcePath}";

                    break;
                case SourceType.None:
                    output = $"Type={SourceType}";

                    break;
                default:

                    throw new ArgumentException("Value is not supported", nameof(SourceType));
            }

            return output;
        }

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        public SourceInformation() : this(null)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger">Optional NLog Logger</param>
        public SourceInformation(Logger logger = null)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();

            _logger.Trace("Entering");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="sourcePath"></param>
        /// <param name="isDirectory"></param>
        /// <param name="repositoryName"></param>
        /// <param name="branchName"></param>
        /// <param name="logger">Optional NLog Logger</param>
        public SourceInformation(
            SourceType sourceType,
            string sourcePath,
            bool isDirectory,
            string repositoryName = null,
            string branchName = null,
            Logger logger = null) : this(logger)
        {
            Validators.AssertIsNotNullOrWhitespace(sourcePath, nameof(sourcePath));

            if (sourceType == SourceType.TfsGit)
            {
                Validators.AssertIsNotNullOrWhitespace(repositoryName, nameof(repositoryName));
                Validators.AssertIsNotNullOrWhitespace(branchName, nameof(branchName));
            }

            SourceType = sourceType;
            SourcePath = sourcePath;
            IsDirectory = isDirectory;
            GitRepositoryName = repositoryName;
            GitBranch = branchName;

            AssertIsValid();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor copies attributes of an existing item
        /// </summary>
        /// <param name="input"></param>
        /// <param name="logger">Optional NLog Logger</param>
        public SourceInformation(SourceInformation input, Logger logger = null) : this(logger)
        {
            Validators.AssertIsNotNull(input, nameof(input));
            input.AssertIsValid();

            SourceType = input.SourceType;
            SourcePath = input.SourcePath;
            IsDirectory = input.IsDirectory;
            GitBranch = input.GitBranch;
            GitRepositoryName = input.GitRepositoryName;
        }

        #endregion Constructors
    }
}