using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using NLog;
using AdoTools.Common.Entities;

// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

namespace AdoTools.Ado
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides access to Azure DevOps Policy Information
    /// </summary>
    public class TaskGroupTool : WebApiBase
    {
        #region Constuctors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public TaskGroupTool(
            VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(vsTsTool, logger, performanceConfiguration)
        {
        }

        #endregion Constuctors

        // See this for the REST API: https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/taskgroups?view=azure-devops-rest-4.1

        /// <summary>
        ///     Get a collection of Task Groups
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaskGroupDefinition> GetTaskGroups()
        {
            var output = new List<TaskGroupDefinition>();

            // https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/taskgroups/list?view=azure-devops-rest-4.1
            // ReSharper disable CommentTypo
            // GET https://dev.azure.com/{organization}/{project}/_apis/distributedtask/taskgroups/{groupId}?expanded={expanded}&taskIdFilter={taskIdFilter}&deleted={deleted}&$top={$top}&continuationToken={continuationToken}&queryOrder={queryOrder}&api-version=4.1-preview.1
            // ReSharper restore CommentTypo

            var uri = MakeUri();

            var result = DoRequest(uri);

            try
            {
                var values = result["value"];

                output.AddRange(values.Select(y => y.ToObject<TaskGroupDefinition>()));
            }
            catch (Exception exception)
            {
                throw new VsTsToolException("Unable to deserialize list of TaskGroups.", exception);
            }

            return output;
        }

        /// <summary>
        ///     Get a single task group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TaskGroupDefinition GetTaskGroup(Guid? id = null)
        {
            TaskGroupDefinition output;

            // https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/taskgroups/list?view=azure-devops-rest-4.1
            // ReSharper disable CommentTypo
            // GET https://dev.azure.com/{organization}/{project}/_apis/distributedtask/taskgroups/{groupId}?expanded={expanded}&taskIdFilter={taskIdFilter}&deleted={deleted}&$top={$top}&continuationToken={continuationToken}&queryOrder={queryOrder}&api-version=4.1-preview.1
            // ReSharper restore CommentTypo

            var uri = MakeUri(id);

            var result = DoRequest(uri);

            try
            {
                var values = result["value"];

                output = values.FirstOrDefault()?.ToObject<TaskGroupDefinition>();
            }
            catch (Exception exception)
            {
                throw new VsTsToolException("Unable to deserialize TaskGroupDefinition.", exception);
            }

            return output;
        }

        private string MakeUri(Guid? groupId = null)
        {
            // ReSharper disable CommentTypo
            // GET https://dev.azure.com/{organization}/{project}/_apis/distributedtask/taskgroups/{groupId}?expanded={expanded}&taskIdFilter={taskIdFilter}&deleted={deleted}&$top={$top}&continuationToken={continuationToken}&queryOrder={queryOrder}&api-version=4.1-preview.1
            // ReSharper restore CommentTypo

            // ReSharper disable StringLiteralTypo
            var path =
                $"{VsTsTool.AzureDevOpsApiUri}/{VsTsTool.ProjectName}/_apis/distributedtask/taskGroups";
            // ReSharper restore StringLiteralTypo

            var query = "";

            if (groupId.HasValue && groupId.Value != Guid.Empty)
            {
                query = $"/{groupId.Value}";
            }

            var output = path + query + "?api-version=5.1-preview.1";

            Logger.Trace("Exiting");

            return output;
        }
    }
}