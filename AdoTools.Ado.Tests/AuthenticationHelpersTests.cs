using System;
using System.IO;
using NUnit.Framework;

namespace AdoTools.Ado.Tests
{
    [TestFixture]
    public static class AuthenticationHelpersTests
    {
        private static string MakeJunkPath(string basePath = "%AppData%")
        {
            string junkPath;

            while (true)
            {
                junkPath = Path.Combine(Environment.ExpandEnvironmentVariables(basePath), Guid.NewGuid().ToString());

                // Verify that the file doesn't already exist. This has happened in the past even though it shouldn't because of using GUID.
                if (!File.Exists(junkPath))
                {
                    break;
                }
            }

            return junkPath;
        }

        [Test]
        public static void GetPersonalAccessToken_Args()
        {
            const string token = "token";
            var args = new[] {token, "junk1"};

            var junkPath = MakeJunkPath();
            var result = AuthenticationHelper.GetPersonalAccessToken(args, junkPath);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(token));
        }

        [Test]
        public static void GetPersonalAccessToken_CustomPath_MissingFile()
        {
            var args = Array.Empty<string>();

            var junkPath = MakeJunkPath();

            Assert.That(
                () => AuthenticationHelper.GetPersonalAccessToken(args, junkPath),
                Throws.InvalidOperationException);
        }

        [Test]
        public static void GetPersonalAccessToken_Default()
        {
            var tokenPath = Environment.ExpandEnvironmentVariables(AuthenticationHelper.DefaultTokenFilePath);

            if (!File.Exists(tokenPath))
            {
                Assert.Inconclusive("This test requires that the token file exists at " + tokenPath);
            }

            var result = AuthenticationHelper.GetPersonalAccessToken();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public static void GetUsername_BlankInput([Values("", "  ", null)] string input)
        {
            Assert.That(() => AuthenticationHelper.GetUsername(input), Throws.ArgumentException);
        }

        [Test]
        public static void GetUsername_Custom_WithBadVariable()
        {
            const string username = "%JunkVariable%@xyz.com";
            var expandedUsername = Environment.ExpandEnvironmentVariables(username);

            var result = AuthenticationHelper.GetUsername(username);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expandedUsername));
        }

        [Test]
        public static void GetUsername_Custom_WithLiteral()
        {
            const string username = "SallySmith@xyz.com";

            var result = AuthenticationHelper.GetUsername(username);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(username));
        }

        [Test]
        public static void GetUsername_Custom_WithVariable()
        {
            const string username = "%USERNAME%@xyz.com";
            var expandedUsername = Environment.ExpandEnvironmentVariables(username);

            var result = AuthenticationHelper.GetUsername(username);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expandedUsername));
        }

        [Test]
        public static void GetUsername_Default()
        {
            var result = AuthenticationHelper.GetUsername();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }
    }
}