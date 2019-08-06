using System;
using System.IO;
using System.Xml.Serialization;

// ReSharper disable UnusedMember.Global

namespace AdoTools.Common.Extensions
{
    /// <summary>
    ///     Extensions for XML
    /// </summary>
    public static class XmlExtensionMethods
    {
        /// <summary>
        ///     Serialize an object using the XmlSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        /// <param name="formatOutput"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(this T toSerialize, bool formatOutput = false) where T : new()
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            string output;

            using (var stream = new MemoryStream())
            {
                xmlSerializer.Serialize(stream, toSerialize);
                stream.Seek(0, 0);
                using (var reader = new StreamReader(stream))
                {
                    output = reader.ReadToEnd();
                }
            }

            if (formatOutput)
            {
                output = XmlTools.FormatXml(output);
            }

            return output;
        }
    }
}