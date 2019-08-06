using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using AdoTools.Common.Extensions;

namespace AdoTools.Common.Tests.Extensions
{
    [TestFixture]
    public static class DictionaryExtensionTests
    {
        [Test]
        public static void Tuples_ReturnsExpectedResult()
        {
            var input = new Dictionary<int, string> {{1, "a"}, {2, "b"}};

            var result = input.Tuples().ToArray();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(input.Count));
        }
    }
}
