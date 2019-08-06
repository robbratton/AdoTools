using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.Policy.WebApi;
using NLog;
using AdoTools.Common;
using AdoTools.Common.Entities;

// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

namespace AdoTools.Ado
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides access to Azure DevOps Policy Information
    /// </summary>
    public class PolicyTool : WaitAndRetryBase
    {
        #region Constuctors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public PolicyTool(VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(vsTsTool, nameof(vsTsTool));

            Logger.Trace("Entering");

            _vsTsTool = vsTsTool;
            _client = _vsTsTool.PolicyClient;
        }

        #endregion Constuctors

        /// <summary>
        ///     Get a Policy Configuration
        /// </summary>
        /// <param name="configurationId"></param>
        /// <returns></returns>
        public PolicyConfiguration GetPolicyConfiguration(int configurationId)
        {
            Logger.Trace("Entering");

            var info = $"{nameof(configurationId)}: {configurationId}";

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                        _ =>
                            _client.Value.GetPolicyConfigurationAsync(_vsTsTool.ProjectName, configurationId)
                        ,
                        MakeContext(info)
                    )
                    .Result;

            HandlePolicyResult(result, info);

            var output = result.Result;

            return output;
        }

        /// <summary>
        ///     Get All Policy Configurations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PolicyConfiguration> GetPolicyConfigurations()
        {
            Logger.Trace("Entering");

            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                        _ =>
                            _client.Value.GetPolicyConfigurationsAsync(_vsTsTool.ProjectName)
                        , MakeContext()
                    )
                    .Result;

            HandlePolicyResult(result);

            var output = result.Result;

            return output;
        }

        #region Private Properties and Fields

        private readonly Lazy<PolicyHttpClient> _client;

        private readonly VsTsTool _vsTsTool;

        #endregion Private Properties and Fields
    }
}