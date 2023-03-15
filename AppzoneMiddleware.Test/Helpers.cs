using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AppzoneMiddleware.Test
{
    public static class Helpers
    {
        public static string ToXmlString<T>(this T input)
        {
            using (var writer = new StringWriter())
            {
                input.ToXml(writer);
                return writer.ToString();
            }
        }

        public static void ToXml<T>(this T objectToSerialize, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, objectToSerialize);
        }

        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
        }

        //public static void ToObject<T>(this T objectToDeserialize, Stream stream)
        //{
        //}

        public static T ToObjectEntity<T>(string input)
        {
            T retVal = default(T);
            var buffer = Encoding.UTF8.GetBytes(input);
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream(buffer))
            {
                retVal = (T)serializer.Deserialize(stream);
            }
            return retVal;
        }
    }
}
