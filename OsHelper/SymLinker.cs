using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

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
        public static void CreateSymlink(string source, string dest, SymbolicLink isFile)
        {
            var errorCode = CreateSymlinkHelper(source, dest, isFile);
            if (errorCode != 0)
            {
                var error = DecodeErrorCode(errorCode);
                throw new SymLinkerError(
                    $"Failed to create symlink, please check developer mode is turned on or run as administrator.\nWin32 Error Message: {error}. \nFile:{source}");
            }
        }

        public static int CreateSymlinkHelper(string source, string dest, SymbolicLink isFile)
        {
            var success = CreateSymbolicLink(dest, source, (UInt32)isFile);
            if (!success)
            {
                var errorCode = Marshal.GetLastWin32Error();
                return errorCode == 0x000000B7 ? 0 : errorCode; // File exist
            }
            return 0;
        }
        public enum SymbolicLink : UInt32
        {
            AllowUnprevileged = 0x2,
            File = (0x0 | AllowUnprevileged),
            Directory = (0x1 | AllowUnprevileged),
        }

        public static string DecodeErrorCode(int errorCode)
        {
            return new Win32Exception(errorCode).Message;
        }
    }

    /// <summary>
    /// Raised when encounted an error duing symbolic io.
    /// </summary>
    [Serializable]
    public class SymLinkerError : Exception
    {
        public SymLinkerError() : base() { }
        public SymLinkerError(string message) : base(message) { }
        public SymLinkerError(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected SymLinkerError(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
