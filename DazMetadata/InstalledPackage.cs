using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Helpers;

namespace DazPackage
{
    /// <summary>
    /// Represent a zip package installed (ie have an install manifest).
    /// </summary>
    public class InstalledPackage : INotifyPropertyChanged
    {
        public InstalledPackage (FileInfo fileInfo)
        {
            installedManifest = new InstallManifestFile(fileInfo);
            foreach (var metadataFileLocation in installedManifest.MetadataFiles)
            {
                var metadataFilePath = new FileInfo(Path.Combine(installedManifest.UserInstallPath, metadataFileLocation));
                Assets.AddRange(new PackageMetadata(metadataFilePath).Assets.Select(x => new AssetMetadata(x)).ToList());

                foreach (var asset in Assets)
                {
                    if (InstalledCharacter.ContentTypeMatches(asset.ContentType))
                    {
                        var figureLocation = Path.Combine(this.InstalledLocation, asset.Name);
                        var figureImage = FindImage(figureLocation);
                        Characters.Add(new InstalledCharacter(this, asset.Compatibilities)
                        {
                            Path = asset.Name,
                            Image = figureImage,
                        });
                        //Output.Write("Character found: " + asset.Name);
                    }
                    else if (InstalledPose.ContentTypeMatches(asset.ContentType))
                    {
                        var figureLocation = Path.Combine(this.InstalledLocation, asset.Name);
                        var figureImage = FindImage(figureLocation);
                        Poses.Add(new InstalledPose(this, asset.Compatibilities)
                        {
                            Path = asset.Name,
                            Image = figureImage,
                        });
                        //Output.Write("Pose found: " + asset.Name);
                    }
                    else if (InstalledMaterial.ContentTypeMatches(asset.ContentType))
                    {

                    }
                    else if (InstalledFileSkipped.ContentTypeMatches(asset.ContentType))
                    {

                    }
                    else
                    {
                        Output.Write(asset.ContentType, Brushes.Red);
                    }
                }
            }
        }
        public InstalledPackage() { }

        public InstallManifestFile installedManifest { get; set; }

        public string ProductName { get { return installedManifest.ProductName; }}
        public string InstalledLocation { get { return installedManifest.UserInstallPath; } }
        public List<AssetMetadata> Assets { get; set; } = new List<AssetMetadata>(); // File in this package.
        public List<string> Files { get { return installedManifest.Files; } }
        public bool Selected { get => selected; set { selected = value; OnPropertyChanged(); } }
        public AssetTypes AssetTypes { get; set; } = AssetTypes.None;
        public Generation Generation { get; set; } = Generation.None;

        public List<InstalledCharacter> Characters { get; set; } = new List<InstalledCharacter>();
        public List<InstalledPose> Poses { get; set; } = new List<InstalledPose>();

        private bool selected = false;
        private static string FindImage (string assetPath)
        {
            var figureImage = Path.ChangeExtension(assetPath, ".tip.png");
            if (!File.Exists(figureImage))
            {
                figureImage = Path.ChangeExtension(assetPath, ".png");
                if (!File.Exists(figureImage))
                {
                    figureImage = null;
                }
            }

            return figureImage;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
