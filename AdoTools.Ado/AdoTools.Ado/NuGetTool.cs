using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using NLog;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Entities;
using NullLogger = NuGet.Common.NullLogger;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedMember.GlobalVsTsApiBaseUri
// ReSharper disable MemberCanBePrivate.Global

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

namespace Upmc.DevTools.VsTs
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides tools to access NuGet V3 repositories.
    /// </summary>
    public class NuGetTool : WaitAndRetryBase
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="repositoryUri">The V3 repository to be accessed</param>
        /// <param name="includePrerelease">Set to True if pre-release packages will be included, otherwise false.</param>
        /// <param name="username">Optional. Provide a username if authentication is required.</param>
        /// <param name="password">Optional. Provide a password if authentication is required.</param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public NuGetTool(
            string repositoryUri = "https://api.nuget.org/v3/index.json",
            bool includePrerelease = false,
            string username = null,
            string password = null,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNullOrWhitespace(repositoryUri, nameof(repositoryUri));

            Logger.Trace("Entering");

            _includePrerelease = includePrerelease;

            var providers = new List<Lazy<INuGetResourceProvider>>();

            providers.AddRange(Repository.Provider.GetCoreV3());

            var packageSource = new PackageSource(repositoryUri);

            if (!string.IsNullOrWhiteSpace(username) || !string.IsNullOrWhiteSpace(password))
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new ArgumentException("Value must not be null or whitespace.", nameof(username));
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("Value must not be null or whitespace.", nameof(password));
                }

                const bool useClearText = true;

                var usernameToUse = username;
                var passwordToUse = password;

                packageSource.Credentials = new PackageSourceCredential(
                    repositoryUri,
                    usernameToUse,
                    passwordToUse,
                    useClearText,
                    null);
            }

            _sourceRepository = new SourceRepository(packageSource, providers);
        }

        #endregion Constructors

        /// <summary>
        ///     Search for a package
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="exactMatch">If true, the Id must match the package name exactly (ignoring case).</param>
        /// <returns>List of <see cref="IPackageSearchMetadata" /></returns>
        public IEnumerable<IPackageSearchMetadata> Search(
            string packageName,
            int? skip = null,
            int? take = null,
            bool exactMatch = true)
        {
            Validators.AssertIsNotNullOrWhitespace(packageName, nameof(packageName));

            Logger.Trace("Entering");

            var packageMetadataResource = _sourceRepository.GetResourceAsync<PackageMetadataResource>().Result;

            var info = $"{nameof(packageName)}: {packageName}, {nameof(exactMatch)}: {exactMatch}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                    _ =>
                        packageMetadataResource.GetMetadataAsync(
                            packageName,
                            _includePrerelease,
                            false,
                            _sourceCacheContext,
                            NullLogger.Instance,
                            CancellationToken.None),
                    MakeContext(info)
                ).Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            var tempPackageName = packageName;
            if (exactMatch)
            {
                output = output.Where(
                    x => x.Identity.Id.Equals(tempPackageName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (skip.HasValue)
            {
                output = output.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                output = output.Take(take.Value);
            }

            return output;
        }

        /// <summary>
        ///     Checks if a package exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the name was found, false otherwise.</returns>
        public bool PackageExists(string name)
        {
            Validators.AssertIsNotNullOrWhitespace(name, nameof(name));

            Logger.Trace("Entering");

            var result = Search(name);

            return result.Any(
                x => string.Equals(x.Identity.Id, name, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        /// <summary>
        ///     Checks if a package exists.
        /// </summary>
        /// <param name="name">Package Name</param>
        /// <param name="version">Package Version</param>
        /// <returns>True if the name was found, false otherwise.</returns>
        public bool PackageExists(string name, string version)
        {
            Validators.AssertIsNotNullOrWhitespace(name, nameof(name));
            Validators.AssertIsNotNullOrWhitespace(version, nameof(version));

            Logger.Trace("Entering");

            var result = Search(name);

            return result.Any(
                x =>
                    string.Equals(x.Identity.Id, name, StringComparison.InvariantCultureIgnoreCase)
                    &&
                    string.Equals(x.Identity.Version.ToString(), version, StringComparison.InvariantCultureIgnoreCase)
            );
        }

        #region Private Properties and Fields

        /// <summary>
        ///     Indicates Whether to Include Pre Release Packages
        /// </summary>
        private readonly bool _includePrerelease;

        private readonly SourceCacheContext _sourceCacheContext = new SourceCacheContext();

        private readonly SourceRepository _sourceRepository;

        #endregion Private Properties and Fields
    }
}