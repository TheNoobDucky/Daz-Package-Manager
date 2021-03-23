using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace OsHelper
{
    public class SymLinker
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, UInt32 dwFlags);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param> Source file location
        /// <param name="dest"></param> Destination file location including file name
        /// <param name="isFile"></param> is file or folder
        /// <returns> win32 error code or 0 </returns>
        public static int CreateSymlink(string source, string dest, SymbolicLink isFile)
        {
            var success = CreateSymbolicLink(dest, source, (UInt32)isFile);
            if (!success)
            {
                var errorCode =  Marshal.GetLastWin32Error();
                if (errorCode == 0x000000B7) // File already exist
                {
                    return 0;
                }
                return errorCode;
            }
            return 0;
        }
        public enum SymbolicLink : UInt32
        {
            AllowUnprevileged = 0x2,
            File = (0x0| AllowUnprevileged),
            Directory = (0x1|AllowUnprevileged),
        }

        public static string DecodeErrorCode (int errorCode)
        {
            return new Win32Exception(errorCode).Message;
        }
    }
}
