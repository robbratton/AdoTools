using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json.Linq;
using NLog;
using AdoTools.Common;
using AdoTools.Common.Entities;

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

namespace AdoTools.Ado
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides access to Azure DevOps Web API Calls
    /// </summary>
    public class WebApiBase : WaitAndRetryBase
    {
        #region Constuctors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="vsTsTool"></param>
        /// <param name="logger">Optional NLog Logger</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        protected WebApiBase(
            VsTsTool vsTsTool,
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNull(vsTsTool, nameof(vsTsTool));

            Logger.Trace("Entering");

            VsTsTool = vsTsTool;

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $":{VsTsTool.PersonalAccessToken}")));
        }

        #endregion Constuctors

        #region Protected Methods

        /// <summary>
        ///     Do a web request and return a JSON Object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        protected static JObject DoRequest(string uri)
        {
            var response = Client.GetAsync(uri).Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new VsTsToolException($"Failed when calling Azure DevOps. Status Code: {response.StatusCode}");
            }

            JObject output;

            if (response.Content != null)
            {
                var content = response.Content.ReadAsStringAsync().Result;

                if (content.Length == 0)
                {
                    throw new VsTsToolException("Failed when calling Azure DevOps. Empty content was returned.");
                }

                try
                {
                    output = (JObject) JToken.Parse(content);
                }
                catch (Exception exception)
                {
                    throw new VsTsToolException("Failed when calling Azure DevOps. JSON parsing failed.", exception);
                }
            }
            else
            {
                throw new VsTsToolException("Failed when calling Azure DevOps. No content was returned.");
            }

            return output;
        }

        #endregion Protected Methods

        #region Private Properties and Fields

        /// <summary>
        ///     VisualStudio Team Services Tool
        /// </summary>
        protected readonly VsTsTool VsTsTool;

        private static readonly HttpClient Client = new HttpClient();

        #endregion Private Properties and Fields
    }
}