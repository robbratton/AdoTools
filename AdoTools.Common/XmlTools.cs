using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
// ReSharper disable UnusedMember.Global

namespace AdoTools.Common
{
    /// <summary>
    ///     Tool for processing XML
    /// </summary>
    public static class XmlTools
    {
        // todo Transform *.x.config files using the methods here.

        /// <summary>
        ///     Deserialize a String to an Object of the Specified Type
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="xmlString">XML String to Be Deserialized</param>
        /// <returns></returns>
        public static T DeserializeObjectFromString<T>(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                throw new ArgumentException(nameof(xmlString));
            }

            var serializer = new XmlSerializer(typeof(T));

            T output;

            var byteArray = Encoding.UTF8.GetBytes(xmlString);
            using (var stream = new MemoryStream(byteArray))
            {
                output = (T) serializer.Deserialize(stream);
            }

            return output;
        }

        /// <summary>
        ///     Format XML Into a Pretty Human-Readable Layout
        /// </summary>
        /// <param name="rawStringXml"></param>
        /// <returns></returns>
        public static string FormatXml(string rawStringXml)
        {
            if (string.IsNullOrWhiteSpace(rawStringXml))
            {
                throw new ArgumentException(nameof(rawStringXml));
            }

            string output;

            var xmlDoc = new XmlDocument();
            using (var sw = new StringWriter())
            {
                xmlDoc.LoadXml(rawStringXml);
                xmlDoc.Save(sw);
                output = sw.ToString().Replace("utf-16", "utf-8");
            }

            return output;
        }
    }
}