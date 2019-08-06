using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

// ReSharper disable PossibleMultipleEnumeration

namespace AdoTools.VsParser.Tests
{
    public static class Helpers
    {
        public static void AssertHasCountOf(IEnumerable<object> collection, int expectedValue)
        {
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Count(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Read the content of an embedded test file.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public static string ReadEmbeddedResourceFile(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException("Value must not be null or whitespace.", nameof(resourceName));
            }

            string output;

            var assembly = Assembly.GetExecutingAssembly();

            var resourcePath = $"AdoTools.VsParser.Tests.TestFiles.{resourceName}";

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource file {resourcePath} was not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    output = reader.ReadToEnd();
                }
            }

            return output;
        }

        /// <summary>
        /// Read the content of a file in the TestFiles folder.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public static string ReadResourceFile(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException("Value must not be null or whitespace.", nameof(resourceName));
            }

            var assembly = Assembly.GetExecutingAssembly();

            var resourcePath = Path.GetDirectoryName(assembly.Location) + @"\TestFiles\AssemblyInfo.cs.txt";

            if (!File.Exists(resourcePath))
            {
                throw new FileNotFoundException($"Resource file {resourcePath} was not found.");
            }

            var output = File.ReadAllText(resourcePath);

            return output;
        }

    }
}