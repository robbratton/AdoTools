using System;
using System.IO;
using System.Net;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace AdoTools.Common
{
    /// <summary>
    ///     Tool for Performing Web Requests
    /// </summary>
    public static class WebTool
    {
        /// <summary>
        ///     Run a Web Request and return the content as a string.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="basicAccessToken"></param>
        /// <returns></returns>
        public static string DoWebRequest(string url, string basicAccessToken = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Value must not be null or whitespace.", nameof(url));
            }

            var request = WebRequest.Create(url);
            request.Method = "GET";
            if (basicAccessToken != null)
            {
                var encodedToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{basicAccessToken}"));
                request.Headers.Add(HttpRequestHeader.Authorization, $"Basic {encodedToken}");
            }

            var response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException(
                    $"Expected response code {HttpStatusCode.OK}, but response code was {response.StatusCode}.");
            }

            var stream = response.GetResponseStream();
            if (stream == null)
            {
                throw new InvalidOperationException("No response stream was available.");
            }

            string output;
            using (var reader = new StreamReader(stream))
            {
                output = reader.ReadToEnd();
            }

            return output;
        }
    }
}