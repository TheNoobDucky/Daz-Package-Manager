using OsHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace DazPackage
{
    public class VirtualPackage
    {
        /// <summary>
        /// Create a virtual copy of packages at desitination.
        /// </summary>
        /// <param name="package">Packages to be installed.</param>
        /// <param name="destination">Destination folder.</param>
        /// <exception cref="SymlinkError">Error when creating the virtual file link.</exception>
        public static void Install(InstalledPackage package, string destination, bool makeCopy = false)
        {
            var basePath = package.InstalledLocation;
            foreach (var file in package.Files)
            {
                var sourcePath = Path.GetFullPath(Path.Combine(basePath, file));
                var destinationPath = Path.GetFullPath(Path.Combine(destination, file));
                Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);
                if (makeCopy)
                {
                    try
                    {
                        File.Copy(sourcePath, destinationPath, false);
                    }
                    catch (IOException error)
                    {
                        if (error.HResult != -2147024816) // ignore fileExist 0x80070050
                        {
                            Output.Write(file + " : " + error.Message, Output.Level.Warning);
                        }
                    }
                } else
                {
                    SymLinker.CreateSymlink(sourcePath, destinationPath, SymLinker.SymbolicLink.File);
                }
            }
        }

        public static void SaveInstallScript(string scriptLocation, string virtualFolder, string sceneFile)
        {
            virtualFolder = virtualFolder.Replace('\\' , '/');
            sceneFile = sceneFile.Replace('\\', '/');
            string script_template =
                "(function() { \n" +
                $"\tvar virtualFolder = \"{virtualFolder}\";\n" +
                $"\tvar sceneFile = \"{sceneFile}\";\n" +
                "\tvar contentManager = App.getContentMgr();\n" +
                "\t//contentManager.removeAllContentDirectories();\n" +
                "\tcontentManager.addContentDirectory(virtualFolder);\n" +
                "\tcontentManager.openFile(sceneFile, false)\n" +
                "})();";

            File.WriteAllText(scriptLocation, script_template);
        }
    }

}
