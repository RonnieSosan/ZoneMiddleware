using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class UploadRequest : BaseRequest
    {
        public UploadType UploadType { get; set; }
        public string AccountNumber { get; set; }
        public string UtilityBill { get; set; }
        public string Picture { get; set; }
        public string NationalIdentity { get; set; }
        public string IDNO { get; set; }
        public string Signature { get; set; }
    }

    public enum UploadType
    {
        NationalID = 1,
        PictureUpload = 2,
        UtilityBill = 3,
        Signature = 4
    }

    public class UploadResponse : BaseResponse { }
}
