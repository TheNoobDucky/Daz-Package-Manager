using System;
using System.Collections.Generic;
using System.Text;
using DazPackage;
using Helpers;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.ComponentModel;

namespace Daz_Package_Manager
{
    class VirtualInstall
    {
        public static void InstallPackages (IEnumerable<InstalledPackage> packages, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var package in packages)
            {
                var basePath = package.InstalledLocation;
                Output.Write("Installing: " + package.ProductName);
                foreach (var file in package.Files)
                {
                    var sourcePath = Path.GetFullPath(Path.Combine(basePath, file));
                    var destinationPath = Path.GetFullPath(Path.Combine(destination, file));
                    Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);
                    var errorCode = SymLinker.CreateSymlink(sourcePath, destinationPath, SymLinker.SymbolicLink.File);
                    if (errorCode != 0)
                    {
                        var error = new Win32Exception(errorCode).Message;
                        MessageBox.Show("Failed to create symlink. Aborting. Win32 Error message:" + error);
                        return;
                    }
                }
            }
        }
    }

    class SymLinker
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
            }
            return 0;
        }
        public enum SymbolicLink : UInt32
        {
            AllowUnprevileged = 0x2,
            File = (0x0| AllowUnprevileged),
            Directory = (0x1|AllowUnprevileged),
        }
    }
}
