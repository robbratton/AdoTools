using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using NLog;
using AdoTools.Common.Entities;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

namespace AdoTools.Ado
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides access to Azure DevOps Policy Information
    /// </summary>
    public class VariableGroupTool : WebApiBase
    {
        /* See this for the REST API: https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/variablegroups/get%20variable%20groups?view=azure-devops-rest-4.1#get_variable_groups_using_group_name_filter
         does this have access? Microsoft.TeamFoundationServer.ExtendedClient? https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=vsts
         */
        /// <summary>
        ///     Get a list of Variable Groups
        /// </summary>
        /// <returns>Dictionary of Id and Name</returns>
        public IEnumerable<VariableGroup> GetVariableGroups()
        {
            var output = new List<VariableGroup>();

            // ReSharper disable CommentTypo
            // API Document: https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/variablegroups/get%20variable%20groups?view=azure-devops-rest-4.1
            // Example: GET https://dev.azure.com/{organization}/{project}/_apis/distributedtask/variablegroups?api-version=4.1-preview.1
            // ReSharper restore CommentTypo

            var uri = MakeUri();

            var result = DoRequest(uri);

            try
            {
                var values = result["value"];

                output.AddRange(values.Select(y => y.ToObject<VariableGroup>()));
            }
            catch (Exception exception)
            {
                throw new VsTsToolException("Unable to deserialize list of VariableGroups.", exception);
            }

            return output;
        }

        /// <summary>
        ///     Get a list of Variable Groups
        /// </summary>
        /// <returns>Dictionary of Id and Name</returns>
        public IDictionary<int, string> GetVariableGroupNames()
        {
            var values = GetVariableGroups();

            return values.ToDictionary(item => item.Id, item => item.Name);
        }

        /// <summary>
        ///     Get the ID for a variableGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns>Group Id</returns>
        /// <throws>
        ///     If the groupName is not found, throws InvalidOperationException with the message "Sequence contains no matching
        ///     element"
        /// </throws>
        public int GetVariableGroupId(string groupName)
        {
            var groups = GetVariableGroupNames();

            // This will throw an exception if the group is not found. We don't want to use FirstOrDefault because we don't want to return 0.
            var groupId = groups.Single(x => x.Value.Equals(groupName, StringComparison.CurrentCultureIgnoreCase))
                .Key;

            return groupId;
        }

        /// <summary>
        ///     Get a list of Variable Names for a Variable Group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>Collection of Names</returns>
        public IEnumerable<string> GetVariableNames(int groupId)
        {
            var output = new List<string>();

            // ReSharper disable CommentTypo
            // API Document: https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/variablegroups/get?view=azure-devops-rest-4.1
            // Example: GET https://dev.azure.com/{organization}/{project}/_apis/distributedtask/variablegroups/{groupId}?api-version=4.1-preview.1
            // ReSharper enable CommentTypo

            var uri = MakeUri(groupId);

            var result = DoRequest(uri);

            try
            {
                var variables = result["variables"].ToObject<Dictionary<string, object>>();

                foreach (var variable in variables)
                {
                    var item = variable.Key;

                    output.Add(item);
                }
            }
            catch (Exception exception)
            {
                throw new VsTsToolException("Unable to deserialize list of variable names.", exception);
            }

            return output;
        }

        #region Constuctors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public VariableGroupTool(
            VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(vsTsTool, logger, performanceConfiguration)
        {
        }

        #endregion Constuctors

        private string MakeUri(int? groupId = null)
        {
            // ReSharper disable StringLiteralTypo
            var path =
                $"{VsTsTool.AzureDevOpsApiUri}/{VsTsTool.ProjectName}/_apis/distributedtask/variablegroups";
            // ReSharper restore StringLiteralTypo

            var query = "";

            if (groupId.HasValue)
            {
                query = $"/{groupId}";
            }

            var output = path + query + "?api-version=5.1-preview.1";

            Logger.Trace("Exiting");

            return output;
        }
    }
}