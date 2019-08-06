using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using NLog;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Entities;
using Upmc.DevTools.VsTs.Entities;

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Upmc.DevTools.VsTs.SourceTools
{
    /// <inheritdoc cref="WaitAndRetryBase" />
    public class GitSourceTool : WaitAndRetryBase, ISourceTool<GitItem>
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public GitSourceTool(
            VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(vsTsTool, nameof(vsTsTool));

            Logger.Trace("Entering");

            _vsTsTool = vsTsTool;
            _client = _vsTsTool.GitHttpClient;

            _repositoryIdCache = new ConcurrentDictionary<string, Guid>();

            Logger.Trace("Exiting");
        }

        #endregion Constructors

        #region Interface Implementation

        /// <inheritdoc />
        [Obsolete("Not implemented yet.")]
        public void ChangeItemContent(SourceInformation sourceInformation, GitItem existingItem, string content)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            sourceInformation.AssertIsValid();

            // todo Create a commit.
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public GitItem GetItem(SourceInformation searchInformation)
        {
            searchInformation.AssertIsValid();
            Validators.AssertIsNotNull(_client, "GitHttpClient");

            Logger.Trace("Entering");

            var info = $"{nameof(searchInformation)}: {searchInformation}";

            var result =
                WaitAndRetryPolicy.ExecuteAndCapture(
                    _ => IgnoreExceptionsHelper.DoMethodIgnoringExceptions<GitItem>(
                        new Func<SourceInformation, GitItem>(GetItemWrapper),
                        new[] {typeof(VssServiceException)},
                        searchInformation
                    ),
                    MakeContext(info));

            HandlePolicyResult(result, info);

            var output = result.Result;

            Logger.Trace("Exiting");

            return output;
        }

        /// <inheritdoc />
        public string GetItemContent(SourceInformation sourceInformation)
        {
            sourceInformation.AssertIsValid();
            Validators.AssertIsNotNull(_client, "GitHttpClient");

            Logger.Trace("Entering");

            string output = null;

            var info = $"{nameof(sourceInformation)}: {sourceInformation}";

            var result =
                WaitAndRetryPolicy.ExecuteAndCapture(
                    _ => IgnoreExceptionsHelper.DoMethodIgnoringExceptions<Stream>(
                        new Func<SourceInformation, Stream>(GetItemContentWrapper),
                        new[] {typeof(VssServiceException)},
                        sourceInformation
                    ),
                    MakeContext(info));

            HandlePolicyResult(result, info);

            var contentStream = result.Result;

            if (contentStream != null)
            {
                var reader = new StreamReader(contentStream);
                output = reader.ReadToEnd();
            }

            Logger.Trace("Exiting");

            return output;
        }

        // todo Make a version of this for searching?
        /// <inheritdoc />
        public IEnumerable<GitItem> GetItems(SourceInformation searchInformation)
        {
            Validators.AssertIsNotNull(searchInformation, nameof(searchInformation));
            searchInformation.AssertIsValid();

            Logger.Trace("Entering");

            var info = $"{nameof(searchInformation)}: {searchInformation}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                    _ => _client.Value.GetItemsAsync(
                        includeContentMetadata: true,
                        project: _vsTsTool.ProjectName,
                        repositoryId: searchInformation.GitRepositoryName,
                        recursionLevel: VersionControlRecursionType.Full,
                        versionDescriptor: MakeVersionDescriptor(searchInformation)
                    ),
                    MakeContext(info)
                ).Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            Logger.Trace("Exiting");

            return output;
        }

        /// <inheritdoc />
        public SourceInformation Map(GitItem input, string repositoryName, string branchName)
        {
            Logger.Trace("Entering");

            var sourceInformation = new SourceInformation(
                SourceType.TfsGit,
                input.Path,
                input.IsFolder,
                repositoryName,
                branchName);

            Logger.Trace("Exiting");

            return sourceInformation;
        }

        /// <inheritdoc />
        public void UpdateSourceInformation(SourceInformation sourceInformation, GitItem item)
        {
            Validators.AssertIsNotNull(sourceInformation, nameof(sourceInformation));
            Validators.AssertIsNotNull(item, nameof(item));

            Logger.Trace("Entering");

            sourceInformation.SourcePath = item.Path;

            Logger.Trace("Exiting");
        }

        internal Stream GetItemContentWrapper(SourceInformation sourceInformation)
        {
            Logger.Trace("Entering");

            var output = _client.Value.GetItemContentAsync(
                    includeContentMetadata: true,
                    project: _vsTsTool.ProjectName,
                    repositoryId: sourceInformation.GitRepositoryName,
                    path: sourceInformation.SourcePath,
                    recursionLevel: VersionControlRecursionType.None,
                    versionDescriptor: MakeVersionDescriptor(sourceInformation)
                )
                .Result;

            Logger.Trace("Exiting");

            return output;
        }

        internal GitItem GetItemWrapper(SourceInformation sourceInformation)
        {
            Logger.Trace("Entering");

            var output = _client.Value.GetItemAsync(
                    includeContentMetadata: true,
                    project: _vsTsTool.ProjectName,
                    repositoryId: sourceInformation.GitRepositoryName,
                    path: sourceInformation.SourcePath,
                    recursionLevel: VersionControlRecursionType.None,
                    versionDescriptor: MakeVersionDescriptor(sourceInformation)
                )
                .Result;

            Logger.Trace("Exiting");

            return output;
        }

        private static GitVersionDescriptor MakeVersionDescriptor(SourceInformation searchInformation)
        {
            Logger.Trace("Entering");

            var output = new GitVersionDescriptor
            {
                VersionType = GitVersionType.Branch,
                Version = searchInformation.GitBranch
            };

            Logger.Trace("Exiting");

            return output;
        }

        #endregion Interface Implementation

        #region Git-Specific Methods

        /// <summary>
        ///     Gets a collection of branches for this repository.
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <returns>Collection of <see cref="GitRef" /></returns>
        public IEnumerable<GitRef> GetBranches(string repositoryName)
        {
            Validators.AssertIsNotNullOrWhitespace(repositoryName, nameof(repositoryName));

            Logger.Trace("Entering");

            var repositoryId = GetRepositoryId(repositoryName);

            var output = GetBranches(repositoryId);

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     Gets a collection of branches for this repository.
        /// </summary>
        /// <param name="repositoryId"></param>
        /// <returns>Collection of <see cref="GitRef" /></returns>
        public IEnumerable<GitRef> GetBranches(Guid repositoryId)
        {
            Validators.AssertIsNotNull(repositoryId, nameof(repositoryId));

            Logger.Trace("Entering");

            var info = $"{nameof(repositoryId)}: {repositoryId}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                    _ => _client.Value.GetBranchRefsAsync(repositoryId),
                    MakeContext(info)).Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     List repositories with ignored repositories removed.
        /// </summary>
        /// <param name="repositoriesToIgnore">A collection of Regex strings for repository names to ignore.</param>
        /// <returns></returns>
        public IEnumerable<GitRepository> GetRepositories(IEnumerable<string> repositoriesToIgnore = null)
        {
            Logger.Trace("Entering");

            IEnumerable<Regex> regularExpressions = null;

            if (repositoriesToIgnore != null)
            {
                regularExpressions = repositoriesToIgnore.Select(r => new Regex(r, RegexOptions.IgnoreCase));
            }

            var output = GetRepositories(regularExpressions);

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     List repositories with ignored repositories removed.
        /// </summary>
        /// <param name="repositoriesToIgnore">A collection of Regex for repository names to ignore.</param>
        /// <returns></returns>
        public IEnumerable<GitRepository> GetRepositories(IEnumerable<Regex> repositoriesToIgnore = null)
        {
            Logger.Trace("Entering");

            var info = $"RepositoriesToIgnore: {(repositoriesToIgnore == null ? "Null" : "Not Null")}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                    _ => _client.Value.GetRepositoriesAsync(_vsTsTool.ProjectName),
                    MakeContext(info)
                ).Result;

            HandlePolicyResult(result, info);

            var results = result.Result;

            var output = results
                .Where(x => repositoriesToIgnore?.All(y => !y.IsMatch(x.Name)) != false);

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     Get the Id of a repository by its name.
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public Guid GetRepositoryId(string repositoryName)
        {
            Logger.Trace("Entering");

            Guid output;

            Interlocked.Increment(ref _repositoryIdCacheSearchCount);

            if (_repositoryIdCache.ContainsKey(repositoryName))
            {
                //Logger.Debug($"Cache hit for {repositoryName}.");
                Interlocked.Increment(ref _repositoryIdCacheHitCount);
                output = _repositoryIdCache[repositoryName];
            }
            else
            {
                //Logger.Debug($"Cache miss for {repositoryName}. Looking up value.");

                var info = $"{nameof(repositoryName)}: {repositoryName}";

                var result =
                    WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                        _ => _client.Value.GetRepositoryAsync(
                            _vsTsTool.ProjectName,
                            repositoryName
                        ),
                        MakeContext(info)).Result;

                HandlePolicyResult(result, info);

                output = result.Result.Id;

                _repositoryIdCache.AddOrUpdate(repositoryName, output, (_, v) => v);
            }

            //Logger.Debug($"Repository Cache for {repositoryName}: Hits {_repositoryIdCacheHitCount * 100.0 / _repositoryIdCacheSearchCount:0.00}% (Hits {_repositoryIdCacheHitCount}, Searches {_repositoryIdCacheSearchCount})");

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     Get Tags
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public IEnumerable<GitRef> GetTags(string repositoryName)
        {
            Logger.Trace("Entering");

            var repositoryId = GetRepositoryId(repositoryName);

            var info = $"{nameof(repositoryName)}: {repositoryName}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                    _ =>
                        _client.Value.GetTagRefsAsync(repositoryId),
                    MakeContext(info)).Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     Gets commits for the repository and branch
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <param name="branchName"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public IEnumerable<GitCommitRef> GetCommits(
            string repositoryName,
            string branchName,
            int? skip = null,
            int? top = null)
        {
            Logger.Trace("Entering");

            var repositoryId = GetRepositoryId(repositoryName);

            var info = $"{nameof(repositoryName)}: {repositoryName}";

            var searchCriteria = new GitQueryCommitsCriteria
            {
                ItemVersion = new GitVersionDescriptor
                {
                    VersionType = GitVersionType.Branch,
                    Version = branchName
                }
            };

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                    _ =>
                        _client.Value.GetCommitsAsync(_vsTsTool.ProjectName, repositoryId, searchCriteria, skip, top),
                    MakeContext(info)).Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            Logger.Trace("Exiting");

            return output;
        }

        #endregion Git-Specific Methods

        #region WebApi Version

        // Endpoint Example: https://dev.azure.com/upmcappsvcs/_apis/git
        // See https://docs.microsoft.com/en-us/rest/api/vsts/git/

        /// <inheritdoc />
        /// <summary>
        ///     Delete an Existing Item
        /// </summary>
        /// <param name="sourceInformation"></param>
        /// <param name="existingItem"></param>
        [Obsolete("Old WebApi Version")]
        public void DeleteItem(SourceInformation sourceInformation, GitItem existingItem)
        {
            throw new NotImplementedException();
        }

        internal string MakeRepoVsTsUri(
            string repositoryName = null,
            string pathSuffix = null,
            string querySuffix = null)
        {
            // ReSharper disable once StringLiteralTypo
            var path = $"{_vsTsTool.AzureDevOpsApiUri}/{_vsTsTool.ProjectName}/_apis/git/repositories";

            if (!string.IsNullOrWhiteSpace(repositoryName))
            {
                path += $"/{repositoryName}";
            }

            if (!string.IsNullOrWhiteSpace(pathSuffix))
            {
                path += $"/{pathSuffix}";
            }

            var query = $"?api-version={GitSourceToolApiVersion}";

            if (!string.IsNullOrWhiteSpace(querySuffix))
            {
                query += $"&{querySuffix}";
            }

            var output = path + query;

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     The default API Version to Use
        /// </summary>
        //public const string ApiVersion = "2.0-preview.1";
        public const string GitSourceToolApiVersion = "5.0-preview.1";

        #endregion WebApi Version

        #region Private Properties and Fields

        private readonly Lazy<GitHttpClient> _client;

        private readonly ConcurrentDictionary<string, Guid> _repositoryIdCache;
        private long _repositoryIdCacheHitCount;
        private long _repositoryIdCacheSearchCount;

        private readonly VsTsTool _vsTsTool;

        #endregion Private Properties and Fields
    }
}