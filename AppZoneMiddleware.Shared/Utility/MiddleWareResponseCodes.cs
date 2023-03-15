using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility
{
    /// <summary>
    /// This captures all the response codes supported by the Providus Middleware. 
    /// The rule for categorization is that response code value of "00" mean "Successful", 
    /// response code values between 01 and 999 are Failures (Due to System Malfunction), 
    /// while response code values from 1000 and above, are Failures (Due to User Action/Data).
    /// </summary>
    public enum MiddleWareResponseCodes
    {
        #region Successful Code
        Successful = 00,
        #endregion

        #region System Error Codes
        ReferenceNumberExists = 01,
        InvalidBranchCode = 02,
        SuccessfulBillsPaymentReversal = 03,
        FailedBillsPaymentReversal = 04,
        SystemError = 06,
        SystemConfigurationError = 07,
        InvalidSystemCredentialsOnAD = 14,
        IssuerOrSwitchInoperative = 91,
        InvalidRequestPayload = 99,
        MissingCustomerAccountInformation = 100,
        MissingEmailRequestData = 101,
        #endregion

        #region User/Client/Data Related Error Codes
        ISECServiceAuthorizationFailure = 1001,
        AccountNotFound = 1010,
        NoResult = 1011,
        AccountValidationFailed = 1012,
        DuplicateTransactionDetected = 1013,
        InvalidUserCredentialsOnAD = 1014,
        InvalidTransactionAmount = 1015,
        MultiCurrencyTransferNotAllowed = 1016,
        EmailAddressNotFound = 1017,
        ProfileLocked = 1018,
        InvalidBVN = 1019,
        CustomerIDMisMatch = 1020,
        TokenExpired = 1023,
        InvalidToken = 1024,
        InvalidCustomerID = 1026,
        InvalidPassword = 1027,
        InvalidOTP = 1031,
        AlreadyExistingCustomerID = 1033,
        OTPExpired = 1034,
        NonExistentOTP = 1042,
        InvalidPin = 1055,
        TransferLimitExceeded = 1061,
        ExceedsWithdrawalFrequency = 1065,
        DisAllowedAccountType = 1066,
        MobifinAirtimeTopUpFailureOnUser = 1067,
        DebitFailed = 1068,
        BetNaijaValidationFailed = 1090,
        BetNaijaFundWalletFailed = 1091,

        #endregion

        #region Third Party API Related Error
        CBA_InvalidResponseFromServer = 3000,
        CBA_JsonSerializationException = 3001,
        Bet9ja_InvalidResponseFromServer = 3002,
        #endregion
    }
}
