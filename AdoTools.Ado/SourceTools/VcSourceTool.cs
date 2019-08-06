using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using NLog;
using AdoTools.Common;
using AdoTools.Common.Entities;
using AdoTools.Ado.Entities;

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global

namespace AdoTools.Ado.SourceTools
{
    /// <inheritdoc cref="WaitAndRetryBase" />
    public class VcSourceTool : WaitAndRetryBase, ISourceTool<TfvcItem>
    {
        #region Private Properties and Fields

        private readonly Lazy<TfvcHttpClient> _client;

        #endregion Private Properties and Fields

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public VcSourceTool(VsTsTool vsTsTool, Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(vsTsTool, nameof(vsTsTool));

            Logger.Trace("Entering");

            _client = vsTsTool.TfVcClient;
        }

        #endregion Constructors

        #region ISourceTool Implementation

        /// <inheritdoc />
        public void ChangeItemContent(SourceInformation sourceInformation, TfvcItem existingItem, string content)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            Validators.AssertIsNotNull(existingItem, nameof(existingItem));
            Validators.AssertIsNotNullOrEmpty(content, nameof(content));

            Logger.Trace("Entering");

            sourceInformation.AssertIsValid();
            sourceInformation.AssertIsSourceType(SourceType.TfsVc);

            var isAdd = existingItem == null;

            var item = new TfvcItem
            {
                Path = sourceInformation.SourcePath,
                ContentMetadata = new FileContentMetadata {Encoding = 65001}
            };

            if (!isAdd)
            {
                item.ChangesetVersion = existingItem.ChangesetVersion;
            }

            var change = new TfvcChange
            {
                ChangeType = isAdd
                    ? VersionControlChangeType.Add
                    : VersionControlChangeType.Edit,
                NewContent = new ItemContent
                {
                    Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
                    ContentType = ItemContentType.Base64Encoded
                },
                Item = item
            };

            var changeset = new TfvcChangeset
            {
                Comment = "Automatically "
                          + (isAdd
                              ? "Added"
                              : "Updated")
                          + " from API",
                Changes = new List<TfvcChange> {change}
                //PolicyOverride = new TfvcPolicyOverrideInfo("API", null),
            };

            // submit the changeset
            var result = _client.Value.CreateChangesetAsync(
                    changeset,
                    VsTsTool.GetProjectNameFromPath(sourceInformation.SourcePath))
                .Result;

            Console.WriteLine($"Changeset created for Add/Update. Id: {result.ChangesetId}.");
        }

        /// <inheritdoc />
        public void DeleteItem(SourceInformation sourceInformation, TfvcItem existingItem)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            Validators.AssertIsNotNull(existingItem, nameof(existingItem));

            sourceInformation.AssertIsValid();
            sourceInformation.AssertIsSourceType(SourceType.TfsVc);

            Logger.Trace("Entering");

            var item = new TfvcItem
            {
                Path = sourceInformation.SourcePath,
                ContentMetadata = new FileContentMetadata {Encoding = 65001},
                ChangesetVersion = existingItem.ChangesetVersion
            };

            var change = new TfvcChange
            {
                ChangeType = VersionControlChangeType.Delete,
                Item = item
            };

            var changeset = new TfvcChangeset
            {
                Comment = "Automatically deleted from API",
                Changes = new List<TfvcChange> {change}
                //PolicyOverride = new TfvcPolicyOverrideInfo("API", null),
            };

            // submit the changeset
            var result = _client.Value.CreateChangesetAsync(
                    changeset,
                    VsTsTool.GetProjectNameFromPath(sourceInformation.SourcePath))
                .Result;

            Console.WriteLine($"Changeset created for delete. Id: {result.ChangesetId}.");
        }

        /// <inheritdoc />
        public TfvcItem GetItem(SourceInformation searchInformation)
        {
            Validators.AssertIsNotNull(searchInformation, nameof(searchInformation));
            searchInformation.AssertIsValid();
            searchInformation.AssertIsSourceType(SourceType.TfsVc);

            Logger.Trace("Entering");

            //var output = _tfVcClient.GetItemAsync(searchInformation.SourcePath).Result;

            var output = IgnoreExceptionsHelper.DoMethodIgnoringExceptions<TfvcItem>(
                new Func<string, TfvcItem>(GetItemWrapper),
                new[] {typeof(VssServiceException)},
                searchInformation.SourcePath
            );

            return output;
        }

        /// <inheritdoc />
        public string GetItemContent(SourceInformation sourceInformation)
        {
            sourceInformation.AssertIsValid();
            sourceInformation.AssertIsSourceType(SourceType.TfsVc);

            Logger.Trace("Entering");

            //var stream = _tfVcClient.GetItemContentAsync(searchInformation.SourcePath).Result;

            var stream =
                IgnoreExceptionsHelper.DoMethodIgnoringExceptions<Stream>(
                    new Func<string, Stream>(GetItemContentAsyncWrapper),
                    new[] {typeof(VssServiceException)},
                    sourceInformation.SourcePath
                );

            string output = null;

            if (stream != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    output = reader.ReadToEnd();
                }
            }

            return output;
        }

        /// <inheritdoc />
        public IEnumerable<TfvcItem> GetItems(SourceInformation searchInformation)
        {
            Validators.AssertIsNotNull(searchInformation, nameof(searchInformation));
            searchInformation.AssertIsValid();
            searchInformation.AssertIsSourceType(SourceType.TfsVc);

            Logger.Trace("Entering");

            // This line is here to cause the call to throw an exception if the sourcePath doesn't exist. 
            // ReSharper disable once UnusedVariable
            var junkValue = _client.Value.GetItemAsync(searchInformation.SourcePath).Result;

            // GetItemsAsync doesn't throw for invalid paths. It just returns an empty list.
            var rawFiles = _client.Value.GetItemsAsync(searchInformation.SourcePath, VersionControlRecursionType.Full)
                .Result;

            return rawFiles;
        }

        /// <inheritdoc />
        public SourceInformation Map(TfvcItem input, string repositoryName = null, string branchName = null)
        {
            Logger.Trace("Entering");

            return new SourceInformation(SourceType.TfsVc, input.Path, input.IsFolder);
        }

        /// <inheritdoc />
        public void UpdateSourceInformation(SourceInformation sourceInformation, TfvcItem item)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            Validators.AssertIsNotNull(item, nameof(item));

            Logger.Trace("Entering");

            sourceInformation.SourcePath = item.Path;
        }

        #endregion ISourceTool Implementation

        #region Private Methods

        private Stream GetItemContentAsyncWrapper(string input)
        {
            Logger.Trace("Entering");

            return _client.Value.GetItemContentAsync(input).Result;
        }

        private TfvcItem GetItemWrapper(string path)
        {
            Logger.Trace("Entering");

            // This simplifies calling the helper method by removing the ambiguity of multiple default values.
            return _client.Value.GetItemAsync(
                    path,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    default(CancellationToken))
                .Result;
        }

        #endregion Private Methods
    }
}