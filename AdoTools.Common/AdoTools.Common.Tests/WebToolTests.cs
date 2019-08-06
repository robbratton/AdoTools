using System;
using System.Net;
using NUnit.Framework;

namespace Upmc.DevTools.Common.Tests
{
    [TestFixture]
    public static class WebToolTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public static void DoWebRequest_Throws_ArgumentException(string uri)
        {
            Assert.That(
                () => WebTool.DoWebRequest(uri),
                Throws.TypeOf<ArgumentException>()
                    .With
                    .Message.Contains("url")
                    .And
                    .Message.Contains("Value must not be null or whitespace.")
            );
        }

        [Category("Integration")]
        [TestCase("https://x.y.z")]
        public static void DoWebRequest_Throws_WebException(string uri)
        {
            Assert.That(
                () => WebTool.DoWebRequest(uri),
                Throws.TypeOf<WebException>()
                    .With.Message
                    .Contains("The remote name could not be resolved")
            );
        }

        [Test]
        [Category("Integration")]
        public static void DoWebRequest_Returns_Result()
        {
            var result = WebTool.DoWebRequest("http://www.google.com", "Token");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }
    }
}