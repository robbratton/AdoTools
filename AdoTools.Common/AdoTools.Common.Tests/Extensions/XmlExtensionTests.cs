using System;
using NUnit.Framework;
using Upmc.DevTools.Common.Extensions;

namespace Upmc.DevTools.Common.Tests.Extensions
{
    [TestFixture]
    public class XmlExtensionTests
    {
        [Test]
        public void SerializeObject_Format()
        {
            var input = new TestObject {TestString = "testing"};

            var result = input.SerializeObject(true);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
            StringAssert.Contains("testing", result);
        }

        [Test]
        public void SerializeObject_NoFormat()
        {
            var input = new TestObject {TestString = "testing"};

            var result = input.SerializeObject();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
            StringAssert.Contains("testing", result);
        }
    }
}