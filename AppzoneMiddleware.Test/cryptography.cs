using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AppzoneMiddleware.Test
{
    public class CryptoGraphy
    {
        [DllImport("lce.dll", EntryPoint = "EncryptString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr EncryptString([MarshalAs(UnmanagedType.LPStr)] string nxtChal, [MarshalAs(UnmanagedType.LPStr)] string passwd);

        public string GenerateEncryptedPassword(string nextChal, string pwd)
        {
            IntPtr intPtr = EncryptString(nextChal, pwd);
            string retVal = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(intPtr);

            return retVal;
        }
    }
}

