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
        public static void Install(InstalledPackage package, string destinationBase, bool makeCopy = false, bool warnMissingFile = false)
        {
            var basePath = package.InstalledLocation;
            foreach (var file in package.Files)
            {
                Install(file, basePath, destinationBase, makeCopy, warnMissingFile);
            }
        }

        public static void Install (string file, string source, string destinationBase, bool makeCopy = false, bool warnMissingFile = false)
        {
            var sourcePath = Path.GetFullPath(Path.Combine(source, file));
            var destinationPath = Path.GetFullPath(Path.Combine(destinationBase, file));
            Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);

            if (!File.Exists(sourcePath))
            {
                if (warnMissingFile)
                {
                    Output.Write($"File missing: {sourcePath}", Output.Level.Warning);
                }
                return;
            }

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
                        Output.Write($"{file} : {error.Message}", Output.Level.Warning);
                    }
                }
            }
            else
            {
                SymLinker.CreateSymlink(sourcePath, destinationPath, SymLinker.SymbolicLink.File);
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
