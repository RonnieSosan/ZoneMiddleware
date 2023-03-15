using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility
{
    /// <summary>
    /// This extends the StringWriter class, allowing users to specify/indicate the desired encoding.
    /// </summary>
    public class DesiredEncodingStringWriter : StringWriter
    {
        private Encoding _desiredEncoding = Encoding.UTF8;

        public DesiredEncodingStringWriter(StringBuilder sb, Encoding desiredEncoding) : base(sb)
        {
            _desiredEncoding = desiredEncoding;
        }

        /// <summary>
        /// Overrides the base class to return the desired encoding as specified by the user. 
        /// </summary>
        public override Encoding Encoding { get { return _desiredEncoding; } }
    }
}
