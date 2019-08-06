using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NLog;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Entities;
using Upmc.DevTools.VsTs.Entities;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

namespace Upmc.DevTools.VsTs.SourceTools
{
    /// <inheritdoc cref="WaitAndRetryBase" />
    /// <summary>
    ///     Tool for Accessing Source Files from the Filesystem
    /// </summary>
    public class FilesystemSourceTool : WaitAndRetryBase, ISourceTool<FileInfo>
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public FilesystemSourceTool(Logger logger = null, PerformanceConfiguration performanceConfiguration = null) :
            base(logger, performanceConfiguration)
        {
        }

        #endregion Constructors

        #region Public Methods

        /// <inheritdoc />
        public IEnumerable<FileInfo> GetItems(SourceInformation searchInformation)
        {
            Logger.Trace("Entering");

            return GetItems(searchInformation, "*.*");
        }

        /// <inheritdoc />
        public FileInfo GetItem(SourceInformation searchInformation)
        {
            Validators.AssertIsNotNull(searchInformation, nameof(searchInformation));
            searchInformation.AssertIsValid();
            searchInformation.AssertIsSourceType(SourceType.Filesystem);

            Logger.Trace("Entering");

            var output = IgnoreExceptionsHelper.DoConstructorIgnoringExceptions<FileInfo>(
                new[] {typeof(FileNotFoundException), typeof(DirectoryNotFoundException)},
                searchInformation.SourcePath
            );

            if (!output.Exists)
            {
                output = null;
            }

            return output;
        }

        /// <inheritdoc />
        public SourceInformation Map(FileInfo input, string repositoryName = null, string branchName = null)
        {
            Logger.Trace("Entering");

            return new SourceInformation(
                SourceType.Filesystem,
                input.FullName,
                (input.Attributes & FileAttributes.Directory) != 0);
        }

        /// <inheritdoc />
        public string GetItemContent(SourceInformation sourceInformation)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();
            sourceInformation.AssertIsSourceType(SourceType.Filesystem);

            Logger.Trace("Entering");

            //var output = File.ReadAllText(searchInformation.SourcePath);

            var output = IgnoreExceptionsHelper.DoMethodIgnoringExceptions<string>(
                new Func<string, string>(File.ReadAllText),
                new[] {typeof(FileNotFoundException), typeof(DirectoryNotFoundException), typeof(IOException)},
                sourceInformation.SourcePath
            );

            return output;
        }

        /// <inheritdoc />
        public void ChangeItemContent(SourceInformation sourceInformation, FileInfo existingItem, string content)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();
            sourceInformation.AssertIsSourceType(SourceType.Filesystem);

            Logger.Trace("Entering");

            File.WriteAllText(sourceInformation.SourcePath, content);
        }

        /// <inheritdoc />
        public void UpdateSourceInformation(SourceInformation sourceInformation, FileInfo item)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();
            sourceInformation.AssertIsSourceType(SourceType.Filesystem);

            Logger.Trace("Entering");

            sourceInformation.SourcePath = Path.Combine(item.DirectoryName ?? ".", item.Name);
        }

        /// <inheritdoc />
        public void DeleteItem(SourceInformation sourceInformation, FileInfo existingItem)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();
            sourceInformation.AssertIsSourceType(SourceType.Filesystem);

            Logger.Trace("Entering");

            File.Delete(sourceInformation.SourcePath);
        }

        /// <summary>
        ///     Get detailed information for all items based on input.
        /// </summary>
        /// <param name="searchInformation"></param>
        /// <param name="pathFilter">A file filter as used by System.IO.Directory.</param>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetItems(SourceInformation searchInformation, string pathFilter)
        {
            Validators.AssertIsNotNull(searchInformation, nameof(searchInformation));
            searchInformation.AssertIsValid();
            searchInformation.AssertIsSourceType(SourceType.Filesystem);

            Logger.Trace("Entering");

            IEnumerable<FileInfo> output;

            if (searchInformation.IsDirectory)
            {
                output = Directory.GetFiles(searchInformation.SourcePath, pathFilter, SearchOption.AllDirectories)
                    .Select(f => new FileInfo(f));
            }
            else
            {
                output = new List<FileInfo> {new FileInfo(searchInformation.SourcePath)};
            }

            return output;
        }

        #endregion Public Methods
    }
}