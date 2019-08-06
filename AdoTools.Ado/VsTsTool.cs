using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using NLog;
using AdoTools.Common;
using AdoTools.Common.Entities;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local

// ReSharper disable MemberCanBePrivate.Global

[assembly: InternalsVisibleTo("AdoTools.Ado.Tests")]

// ReSharper disable UnusedMember.Global

namespace AdoTools.Ado
{
    /// <inheritdoc />
    /// <summary>
    ///     A Tool for Accessing VsTs Features
    /// </summary>
    /// <remarks>REST API Docs: https://docs.microsoft.com/en-us/rest/api/azure/devops/?view=azure-devops-rest-5.0 </remarks>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class VsTsTool : WaitAndRetryBase
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="personalAccessToken"></param>
        /// <param name="organizationName"></param>
        /// <param name="projectName"></param>
        /// <param name="logger">Optional NLog logger.</param>
        /// <param name="performanceConfiguration">Optional Performance Configuration</param>
        public VsTsTool(
            string personalAccessToken,
            // ReSharper disable once StringLiteralTypo
            string organizationName = "upmcappsvcs",
            string projectName = "Apollo",
            Logger logger = null,
            PerformanceConfiguration performanceConfiguration = null
        ) : base(logger, performanceConfiguration)
        {
            Validators.AssertIsNotNullOrWhitespace(personalAccessToken, nameof(personalAccessToken));
            Validators.AssertIsNotNullOrWhitespace(organizationName, nameof(organizationName));
            Validators.AssertIsNotNullOrWhitespace(projectName, nameof(projectName));

            Logger.Trace("Entering");

            OrganizationName = organizationName;
            PersonalAccessToken = personalAccessToken;
            ProjectName = projectName;

            VssConnection = new Lazy<VssConnection>(
                () =>
                {
                    var output = new VssConnection(AzureDevOpsApiUri,
                        new VssBasicCredential(string.Empty, personalAccessToken));
                    // ReSharper disable CommentTypo
                    // Workaround for HTTP Timeouts. See https://github.com/Microsoft/vsts-agent/blob/master/docs/troubleshooting.md#workaround-httptimeoutexception
                    // ReSharper restore CommentTypo
                    output.Settings.MaxRetryRequest = performanceConfiguration?.MaximumRetries ?? 5;
                    output.Settings.SendTimeout = new TimeSpan(0, 0, performanceConfiguration?.RetryDelay ?? 120);
                    return output;
                }
            );

            HttpClient = new Lazy<HttpClient>(() =>
                {
                    var output = new HttpClient();
                    output.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    output.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(
                                $":{PersonalAccessToken}")));

                    return output;
                }
            );

            BuildClient = new Lazy<BuildHttpClient>(() => VssConnection.Value.GetClient<BuildHttpClient>());

            ReleaseClient = new Lazy<ReleaseHttpClient>(() => VssConnection.Value.GetClient<ReleaseHttpClient>());

            GitHttpClient = new Lazy<GitHttpClient>(() => VssConnection.Value.GetClient<GitHttpClient>());

            PolicyClient = new Lazy<PolicyHttpClient>(() => VssConnection.Value.GetClient<PolicyHttpClient>());

            ProjectClient = new Lazy<ProjectHttpClient>(() => VssConnection.Value.GetClient<ProjectHttpClient>());

            TfVcClient = new Lazy<TfvcHttpClient>(() => VssConnection.Value.GetClient<TfvcHttpClient>());

            Logger.Trace("Exiting");
        }

        #endregion Constructors

        #region Lazy Clients

        /// <summary>
        ///     HttpClient for accessing Azure DevOps
        /// </summary>
        public Lazy<HttpClient> HttpClient { get; }

        /// <summary>
        ///     Client for Accessing Build Information
        /// </summary>
        public Lazy<BuildHttpClient> BuildClient { get; }

        /// <summary>
        ///     Client for Accessing Release Information
        /// </summary>
        public Lazy<ReleaseHttpClient> ReleaseClient { get; }

        /// <summary>
        ///     Client for Accessing Git-specific Features
        /// </summary>
        public Lazy<GitHttpClient> GitHttpClient { get; }

        /// <summary>
        ///     Client for Accessing Policy Information
        /// </summary>
        public Lazy<PolicyHttpClient> PolicyClient { get; }

        /// <summary>
        ///     Client for Accessing Project Information
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        public Lazy<ProjectHttpClient> ProjectClient { get; }

        /// <summary>
        ///     The Client.
        ///     <remarks>This has to be virtual for Moq testing.</remarks>
        /// </summary>
        public virtual Lazy<TfvcHttpClient> TfVcClient { get; }

        /// <summary>
        ///     Connection to Visual Studio
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        public Lazy<VssConnection> VssConnection { get; }

        #endregion Lazy Clients

        #region Public Properties

        /// <summary>
        ///     Personal Access Token for Accessing Azure DevOps
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string PersonalAccessToken { get; }

        /// <summary>
        ///     Guid of the Project (Derived from its Name)
        /// </summary>
        // ReSharper disable once PossibleNullReferenceException
#pragma warning disable RCS1202 // Avoid NullReferenceException.
        public Guid ProjectGuid => GetProjects().FirstOrDefault(p => p.Name == ProjectName).Id;
#pragma warning restore RCS1202 // Avoid NullReferenceException.

        /// <summary>
        ///     Project Name
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        ///     The organization Name used for calls to the Azure DevOps and Azure DevOps APIs
        /// </summary>
        public string OrganizationName { get; }

        /// <summary>
        ///     Prefix for calls to the Azure DevOps API
        /// </summary>
        public Uri AzureDevOpsApiUri => new Uri("https://dev.azure.com/" + OrganizationName);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     Gets the name of the project from the source path.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public static string GetProjectNameFromPath(string sourcePath)
        {
            Validators.AssertIsNotNullOrWhitespace(sourcePath, nameof(sourcePath));

            Logger.Trace("Entering");

            var match = GetProjectNameRegex.Match(sourcePath);

            if (!match.Success)
            {
                throw new ArgumentException("Value is an invalid TFS VC path.", nameof(sourcePath));
            }

            Logger.Trace("Exiting");

            return match.Groups[1].Value;
        }

        /// <summary>
        ///     Get a collection of projects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TeamProjectReference> GetProjects()
        {
            Logger.Trace("Entering");

            var result = WaitAndRetryPolicy.ExecuteAndCapture(
                _ =>
                    ProjectClient.Value.GetProjects().Result.ToList(),
                MakeContext());

            HandlePolicyResult(result);

            var output = result.Result;

            Logger.Trace("Exiting");

            return output;
        }

        /// <summary>
        ///     Do a request to the Azure DevOps API and return the response as a string. The string is normally JSON as configured
        ///     in the HttpClient.
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public async Task<string> DoHttpClientRequest(string suffix)
        {
            if (suffix == null)
            {
                throw new ArgumentNullException(nameof(suffix));
            }

            string output;

            var requestUri = AzureDevOpsApiUri + "/_apis/" + suffix + $"?api-version={AzureDevOpsApiVersion}";

            using (var response = await HttpClient.Value.GetAsync(
                requestUri).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                output = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            return output;
        }

        #endregion Public Methods

        #region Private Fields and Properties

        private const string GetProjectNameRegexPattern = @"^\$/([^/]+)/";

        private static readonly Regex GetProjectNameRegex = new Regex(
            GetProjectNameRegexPattern,
            RegexOptions.Compiled);

        private const string AzureDevOpsApiVersion = "5.0-preview.1";

        #endregion Private Fields and Properties
    }
}