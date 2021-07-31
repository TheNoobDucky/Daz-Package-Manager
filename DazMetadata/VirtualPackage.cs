using Output;
using OsHelper;
using System.IO;

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

        public static void Install(string file, string source, string destinationBase, bool makeCopy = false, bool warnMissingFile = false)
        {
            var sourcePath = Path.GetFullPath(Path.Combine(source, file));
            var destinationPath = Path.GetFullPath(Path.Combine(destinationBase, file));
            Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);

            if (!File.Exists(sourcePath))
            {
                if (warnMissingFile)
                {
                    InfoBox.Write($"File missing: {sourcePath}", InfoBox.Level.Warning);
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
                        InfoBox.Write($"{file} : {error.Message}", InfoBox.Level.Warning);
                    }
                }
            }
            else
            {
                SymLinker.CreateSymlink(sourcePath, destinationPath, SymLinker.SymbolicLink.File);
            }
        }

        public static void SaveInstallScript(string scriptLocation, string virtualFolder, string sceneFile, bool clearBaseDirectories =false)
        {
            virtualFolder = virtualFolder.Replace('\\', '/');
            sceneFile = sceneFile.Replace('\\', '/');
            // clearBaseDirectoriesString = clearBaseDirectories ? "//" : "";

            File.WriteAllText(scriptLocation,
$@"(function() {{ 
	var virtualFolder = ""{virtualFolder}"";
	var sceneFile = ""{sceneFile}"";
	var contentManager = App.getContentMgr();
	var clearBaseDirectories = {clearBaseDirectories.ToString().ToLower()};
	if (clearBaseDirectories) 
	{{
		contentManager.removeAllContentDirectories();
	}}
	contentManager.addContentDirectory(virtualFolder);
	contentManager.openFile(sceneFile, false)
}})();");
        }
    }

}
