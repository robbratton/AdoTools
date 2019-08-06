using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.Build.WebApi;
using Newtonsoft.Json;
using NLog;
using Upmc.DevTools.Common;
using Upmc.DevTools.Common.Entities;

// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("Upmc.DevTools.VsTs.Tests")]

namespace Upmc.DevTools.VsTs
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides access to Azure DevOps Queue Information
    /// </summary>
    public class QueueTool : WaitAndRetryBase
    {

        // API Reference: https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/queues/get%20agent%20queues%20by%20ids?view=azure-devops-rest-5.1

        #region Constuctors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public QueueTool(VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(vsTsTool, nameof(vsTsTool));

            Logger.Trace("Entering");

            _vsTsTool = vsTsTool;
            _client = _vsTsTool.HttpClient;
        }

        #endregion Constuctors

        /// <summary>
        ///     Get one by Id
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        public AgentPoolQueue GetQueue(int queueId)
        {
            Logger.Trace("Entering");

            var info = $"{nameof(queueId)}: {queueId}";

            var tempUri = new Uri(
                $"https://dev.azure.com/{_vsTsTool.OrganizationName}/{_vsTsTool.ProjectName}/_apis/distributedtask/queues/?queueIds={queueId}&api-version=5.1-preview.1");
            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                        _ =>
                            _client.Value.GetStringAsync(tempUri)
                        ,
                        MakeContext(info)
                    )
                    .Result;

            HandlePolicyResult(result, info);

            var serializer = new JsonSerializer();
            var responseItem = serializer.Deserialize<ResponseItem>(new JsonTextReader(new StringReader(result.Result)));

            return responseItem.Value.FirstOrDefault();
        }

        /// <summary>
        ///     Get All 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AgentPoolQueue> GetQueues()
        {
            Logger.Trace("Entering");

            var tempUri = new Uri(
                $"https://dev.azure.com/{_vsTsTool.OrganizationName}/{_vsTsTool.ProjectName}/_apis/distributedtask/queues?api-version=5.1-preview.1");
            var result =
                WaitAndRetryPolicyAsync.ExecuteAndCaptureAsync(
                        _ =>
                            _client.Value.GetStringAsync(tempUri)
                        ,
                        MakeContext()
                    )
                    .Result;

            HandlePolicyResult(result);

            var serializer = new JsonSerializer();
            var responseItem = serializer.Deserialize<ResponseItem>(new JsonTextReader(new StringReader(result.Result)));

            return responseItem.Value;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class ResponseItem
        {
            public AgentPoolQueue[] Value { get; set;}
            public int Count { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

        #region Private Properties and Fields

        private readonly Lazy<HttpClient> _client;

        private readonly VsTsTool _vsTsTool;

        #endregion Private Properties and Fields
    }
}