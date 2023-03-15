using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class UserProfile
    {
        [Key]
        public string CustomerID { get; set; }
        public string Username { get; set; }
        public string PIN { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public DateTime TokenExpDate { get; set; }
        public int InvalidTrials { get; set; }
        public bool IsLocked { get; set; }
        /// <summary>
        /// Indicates if a user recently reset his/her password, and hasn't logged in with the Temporary password generated for him/her. 
        /// </summary>
        public bool WasPasswordRecentlyReset { get; set; }
    }

    public class ValidateOTP
    {
        public string CustomerID { get; set; }
        public string OTP { get; set; }
    }

    /// <summary>
    /// Use this class for user profile registration during onboarding
    /// </summary>
    public class UserAccount
    {
        public string CustomerID { get; set; }
        public string Username { get; set; }
        public string PIN { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public string BVN { get; set; }
        public UserOTP userOTP { get; set; }
        public MailRequest MailRequest { get; set; }
        public string Email { get; set; }
        public CreateWalletRequest CreateWalletRequest { get; set; }
        //Defines new account holder or existign account holder onboardign process
        public bool IsExistingAccountHolder { get; set; }
    }
    public class UserOTP
    {
        [Key]
        public string CustomerID { get; set; }
        public string BVN { get; set; }
        public DateTime OTPExpDate { get; set; }
        public string OTP { get; set; }
        public string OTPType { get; set; }
        public string ValidationToken { get; set; }
    }

    public class ChangePasswordRequest : UserAccount
    {
        public string NewPassword { get; set; }
    }
    public class UserProfileResponse : BaseResponse
    {
        public IList<AccountDetails> AccountInformation { get; set; }
        public WalletData WalletDetails { get; set; }
    }
    public class RegisterProfileResponse : BaseResponse
    {
        OpenAccountResponse OpenAccountResponse { get; set; }
    }
    public class ChangePinRequest : UserAccount
    {
        public string NewPin { get; set; }
    }
    public class ForgotPasswordRequest : UserAccount { }
    public class ForgotPINRequest : UserAccount { }
    public class ResetPasswordRequest : UserAccount
    {
        public string NewPassword { get; set; }
    }

    public class UserLoginRequest : BaseRequest
    {

    }

    public class UserLoginResponse : CustomerAccountsResponse
    {
        public string AuthToken { get; set; }

        /// <summary>
        /// Indicates if a user recently reset his/her password, and hasn't logged in with the Temporary password generated for him/her. 
        /// </summary>
        public bool WasPasswordRecentlyReset { get; set; }
    }

    public class ResetPinRequest : UserAccount
    {
        public string NewPin { get; set; }
    }

    public class ValidatePinRequest : BaseRequest
    {
    }

    public class ValidatePinResponse : BaseResponse
    {
    }

    public class LoggedInUsersResponse : BaseResponse
    {
        public List<UserProfile> LoggedInUsers { get; set; }
    }
}
