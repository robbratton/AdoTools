using System;
using NUnit.Framework;
using AdoTools.Common.Tests.Extensions;

namespace AdoTools.Common.Tests
{
    [TestFixture]
    public static class XmlToolsTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public static void DeserializeObjectFromString_Throws_WithInvalidArg(string input)
        {
            Assert.Throws<ArgumentException>(() =>
                XmlTools.DeserializeObjectFromString<TestObject>(input));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public static void FormatXml_Throws(string input)
        {
            Assert.That(() => XmlTools.FormatXml(input), Throws.ArgumentException);
        }

        //[Test]
        //// todo fix this
        //[Ignore("Need sample XSLT")]
        //public void TransformXml_Succeeds()
        //{
        //    var result = XmlTools.TransformXml(UnformattedXml, Xslt);

        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result, Is.EqualTo(TransformedXml));
        //}

        private const string FormattedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestObject xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <TestString>testing</TestString>
</TestObject>";

        private const string UnformattedXml =
            @"<?xml version=""1.0"" encoding=""utf-8""?><TestObject xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><TestString>testing</TestString></TestObject>";

        [Test]
        public static void DeserializeObjectFromString_Succeeds()
        {
            var result = XmlTools.DeserializeObjectFromString<TestObject>(FormattedXml);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestString, Is.EqualTo("testing"));
        }

        [Test]
        public static void FormatXml_Succeeds()
        {
            var result = XmlTools.FormatXml(UnformattedXml);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(FormattedXml));
        }

        // ReSharper disable CommentTypo
        // todo fix this
        //        [Obsolete("This doesn't work yet")]
        // ReSharper disable once CommentTypo
        //        private const string Xslt = @"<xsl:stylesheet version=""1.0"" exclude-result-prefixes=""xalan str""
        //                xmlns:xsl=""http://www.w3.org/1999/XSL/Transform""
        //                xmlns:xalan=""http://xml.apache.org/xalan""
        //                xmlns:str=""xalan://java.lang.String""
        //        >
        // ReSharper restore CommentTypo

        //<xsl:value-of select=""str:replaceAll('testing', 'Another Test')""/>

        //</xsl:stylesheet>";

        //private const string TransformedXml = @"<?xml version=""1.0"" encoding=""utf-8""?><TestObject xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><TestString>Another Test</TestString></TestObject>";
    }
}