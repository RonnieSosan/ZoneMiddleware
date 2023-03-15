using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class SendOtpResponse : BaseResponse
    {
        public string MaskedPhoneNumber { get; set; }
    }


    public class SendOTPRequest : BaseRequest
    {
        public string CustomerID { get; set; }
        public string BVN { get; set; }
        public DateTime OTPExpDate { get; set; }
        public string OTP { get; set; }
        public string OTPType { get; set; }
        public string ValidationToken { get; set; }
        public bool isResend { get; set; }
    }

}
