using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppzoneMiddleware.Test
{
    public class ObjectFormatter
    {
        /// <summary>
        /// Indicates whether the sample is used for request or response
        /// </summary>
        public enum SampleDirection
        {
            Request = 0,
            Response
        }

        /// <summary>
        /// Writes the sample object using formatter.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception is recorded as InvalidSample.")]
        public virtual object WriteSampleObjectUsingFormatter(MediaTypeFormatter formatter, object value, Type type, MediaTypeHeaderValue mediaType)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            if (mediaType == null)
            {
                throw new ArgumentNullException("mediaType");
            }

            object sample = String.Empty;
            MemoryStream ms = null;
            HttpContent content = null;
            try
            {
                if (formatter.CanWriteType(type))
                {
                    ms = new MemoryStream();
                    content = new ObjectContent(type, value, formatter, mediaType);
                    formatter.WriteToStreamAsync(type, value, ms, content, null).Wait();
                    ms.Position = 0;
                    StreamReader reader = new StreamReader(ms);
                    string serializedSampleString = reader.ReadToEnd();
                    if (mediaType.MediaType.ToUpperInvariant().Contains("XML"))
                    {
                        serializedSampleString = TryFormatXml(serializedSampleString);
                    }
                    else if (mediaType.MediaType.ToUpperInvariant().Contains("JSON"))
                    {
                        serializedSampleString = TryFormatJson(serializedSampleString);
                    }

                    sample = new TextSample(serializedSampleString);
                }
                else
                {
                    sample = new InvalidSample(String.Format(
                        CultureInfo.CurrentCulture,
                        "Failed to generate the sample for media type '{0}'. Cannot use formatter '{1}' to write type '{2}'.",
                        mediaType,
                        formatter.GetType().Name,
                        type.Name));
                }
            }
            catch (Exception e)
            {
                sample = new InvalidSample(String.Format(
                    CultureInfo.CurrentCulture,
                    "An exception has occurred while using the formatter '{0}' to generate sample for media type '{1}'. Exception message: {2}",
                    formatter.GetType().Name,
                    mediaType.MediaType,
                    UnwrapException(e).Message));
            }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                }
                if (content != null)
                {
                    content.Dispose();
                }
            }

            return sample;
        }

        internal static Exception UnwrapException(Exception exception)
        {
            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                return aggregateException.Flatten().InnerException;
            }
            return exception;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Handling the failure by returning the original string.")]
        private static string TryFormatJson(string str)
        {
            try
            {
                object parsedJson = JsonConvert.DeserializeObject(str);
                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch
            {
                // can't parse JSON, return the original string
                return str;
            }
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Handling the failure by returning the original string.")]
        private static string TryFormatXml(string str)
        {
            try
            {
                XDocument xml = XDocument.Parse(str);
                return xml.ToString();
            }
            catch
            {
                // can't parse XML, return the original string
                return str;
            }
        }

        public static object WrapSampleIfString(object sample)
        {
            string stringSample = sample as string;
            if (stringSample != null)
            {
                return new TextSample(stringSample);
            }

            return sample;
        }

        private static bool IsFormatSupported(SampleDirection sampleDirection, MediaTypeFormatter formatter, Type type)
        {
            switch (sampleDirection)
            {
                case SampleDirection.Request:
                    return formatter.CanReadType(type);
                case SampleDirection.Response:
                    return formatter.CanWriteType(type);
            }
            return false;
        }

    }
}
