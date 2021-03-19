using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DazPackage
{
    /// <summary>
    /// Represent a zip package installed (ie have an install manifest).
    /// </summary>
    [Serializable]
    public class InstalledPackage
    {
        public InstalledPackage (FileInfo fileInfo)
        {
            installedManifest = new InstallManifestFile(fileInfo);
            foreach (var metadataFileLocation in installedManifest.MetadataFiles)
            {
                var metadataFilePath = new FileInfo(Path.Combine(installedManifest.UserInstallPath, metadataFileLocation));
                Assets.AddRange(new PackageMetadata(metadataFilePath).Assets.Select(x => new AssetMetadata(x)).ToList());
            }
        }
        public InstalledPackage() { }

        public InstallManifestFile installedManifest { get; set; }

        public string ProductName { get { return installedManifest.ProductName; }}
        public string InstalledLocation { get { return installedManifest.UserInstallPath; } }
        public List<AssetMetadata> Assets { get; set; } = new List<AssetMetadata>(); // File in this package.
        public List<string> Files { get { return installedManifest.Files; } }
        public bool Selected { get; set; } = false;
    }
}
