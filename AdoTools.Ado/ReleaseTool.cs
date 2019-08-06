using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using NLog;
using AdoTools.Common;
using AdoTools.Common.Entities;

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

// ReSharper disable UnusedMember.Global
// ReSharper disable MissingXmlDoc

namespace AdoTools.Ado
{
    /// <inheritdoc />
    /// <summary>
    ///     Client for Accessing Release Information
    /// </summary>
    public class ReleaseTool : WaitAndRetryBase
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public ReleaseTool(VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(vsTsTool, nameof(vsTsTool));

            Logger.Trace("Entering");

            _vsTsTool = vsTsTool;
            _client = _vsTsTool.ReleaseClient;
        }

        #endregion Constructors

        /// <summary>
        ///     Get a Release definition.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public ReleaseDefinition GetReleaseDefinition(int definitionId)
        {
            Logger.Trace("Entering");

            var info = $"{nameof(definitionId)}: {definitionId}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                        _ =>
                            _client.Value.GetReleaseDefinitionAsync(
                                _vsTsTool.ProjectGuid,
                                definitionId,
                                PropertyFilters
                            ),
                        MakeContext(info)
                    )
                    .Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            return output;
        }

        /// <summary>
        ///     Get all Release definitions.
        /// </summary>
        /// <param name="definitionName">Name of the definition. Optional.</param>
        /// <param name="path">Path to the Releases. Optional.</param>
        /// <remarks>
        ///     Use GetReleaseDefinitionReferences if you don't need all the fields in the full Release definitions. It is
        ///     faster.
        /// </remarks>
        /// <returns>Collection of <see cref="ReleaseDefinition" /></returns>
        /// <remarks>This returns VariableGroups as empty even if there are variable groups used in the actual definition</remarks>
        public IEnumerable<ReleaseDefinition> GetReleaseDefinitions(
            string definitionName = null,
            string path = null)
        {
            Logger.Trace("Entering");

            var info = $"{nameof(definitionName)}: {definitionName}, {nameof(path)}: {path}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(_ =>
                        {
                            if (definitionName != null)
                            {
                                return _client.Value.GetReleaseDefinitionsAsync(
                                    searchText: definitionName,
                                    isExactNameMatch: true,
                                    path: path,
                                    project: _vsTsTool.ProjectGuid,
                                    expand:
                                    ReleaseDefinitionExpands,
                                    propertyFilters: PropertyFilters
                                );
                            }

                            return _client.Value.GetReleaseDefinitionsAsync(
                                    path: path,
                                    project: _vsTsTool.ProjectGuid,
                                    expand:
                                    ReleaseDefinitionExpands,
                                    propertyFilters: PropertyFilters
                                )
                                ;
                        }, MakeContext(info)
                    )
                    .Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            return output.ToList();
        }

        #region Private Properties and Fields

        private const ReleaseDefinitionExpands ReleaseDefinitionExpands =
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionExpands.Artifacts
            | Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionExpands.Environments
            | Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionExpands.Triggers
            | Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionExpands.Variables;

        private static readonly string[] PropertyFilters =
        {
            "Artifacts",
            "Environments",
            "Properties",
            "Triggers",
            "VariableGroups",
            "Variables"
        };

        private readonly Lazy<ReleaseHttpClient> _client;

        private readonly VsTsTool _vsTsTool;

        #endregion Private Properties and Fields
    }
}