﻿using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IPasswordManager
    {
        Task<UserProfileResponse> ChangePassword(ChangePasswordRequest passwordRequest);

        Task<UserProfileResponse> ForgotPassword(UserAccount forgotPasswordRequest);

        Task<UserProfileResponse> ResetPassword(ResetPasswordRequest resetPasswordRequest);
    }
}
