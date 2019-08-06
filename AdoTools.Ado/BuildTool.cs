using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.Build.WebApi;
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
    ///     Client for Accessing Build Information
    /// </summary>
    public class BuildTool : WaitAndRetryBase
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public BuildTool(
            VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(vsTsTool, nameof(vsTsTool));

            Logger.Trace("Entering");

            _vsTsTool = vsTsTool;
            _client = _vsTsTool.BuildClient;
        }

        #endregion Constructors

        /// <summary>
        ///     Get a build definitions.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public BuildDefinition GetBuildDefinition(int definitionId)
        {
            Logger.Trace("Entering");

            var output = _client.Value.GetDefinitionAsync(
                    _vsTsTool.ProjectGuid,
                    definitionId
                )
                .Result;

            return output;
        }

        /// <summary>
        ///     Get all build definition references.
        /// </summary>
        /// <param name="definitionName">Name of the definition. Optional.</param>
        /// <param name="repositoryId">Name or ID of the repository. Optional.</param>
        /// <param name="path">Path to the builds. Optional.</param>
        /// <remarks>
        ///     Build references contain a subset of the fields in a build definition.
        /// </remarks>
        /// <returns>Collection of <see cref="BuildDefinitionReference" /></returns>
        public IEnumerable<BuildDefinitionReference> GetBuildDefinitionReferences(
            string definitionName = null,
            string repositoryId = null,
            string path = null)
        {
            Logger.Trace("Entering");

            var output = _client.Value.GetDefinitionsAsync(
                    name: definitionName,
                    repositoryId: repositoryId,
                    path: path,
                    project: _vsTsTool.ProjectGuid
                )
                .Result;

            return output;
        }

        /// <summary>
        ///     Get all  build definitions.
        /// </summary>
        /// <param name="definitionName">Name of the definition. Optional.</param>
        /// <param name="repositoryId">Name or ID of the repository. Optional.</param>
        /// <param name="repositoryType"></param>
        /// <param name="path">Path to the builds. Optional.</param>
        /// <remarks>Use GetBuildDefinitionReferences if you don't need all the fields in the full build definitions. It is faster.</remarks>
        /// <returns>Collection of <see cref="BuildDefinition" /></returns>
        public IEnumerable<BuildDefinition> GetBuildDefinitions(
            string definitionName = null,
            string repositoryId = null,
            string repositoryType = null,
            string path = null)
        {
            Logger.Trace("Entering");

            var output = _client.Value.GetFullDefinitionsAsync(
                    name: definitionName,
                    repositoryId: repositoryId,
                    repositoryType: repositoryType,
                    path: path,
                    project: _vsTsTool.ProjectGuid
                )
                .Result;

            return output;
        }

        #region Private Properties and Fields

        private readonly Lazy<BuildHttpClient> _client;

        private readonly VsTsTool _vsTsTool;

        #endregion Private Properties and Fields
    }
}