using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Helpers;
using System;
using SD.Tools.Algorithmia.GeneralDataStructures;

namespace DazPackage
{
    /// <summary>
    /// Represent a zip package installed (ie have an install manifest).
    /// </summary>
    public class InstalledPackage : INotifyPropertyChanged
    {
        public InstalledPackage (FileInfo fileInfo)
        {
            InstalledManifest = new InstallManifestFile(fileInfo);

            foreach (var metadataFileLocation in InstalledManifest.MetadataFiles)
            {
                var metadataFilePath = new FileInfo(Path.Combine(InstalledManifest.UserInstallPath, metadataFileLocation));
                var Assets = new PackageMetadata(metadataFilePath).Assets.Select(x => new AssetMetadata(x)).ToList();

                foreach (var asset in Assets)
                {
                    var assetType = InstalledFile.MatchContentType(asset.ContentType);
                    if ((assetType & AssetTypes.Handled) != AssetTypes.None)
                    {
                        var figureLocation = Path.Combine(this.InstalledLocation, asset.Name);
                        var figureImage = FindImage(figureLocation);
                        var item = new InstalledFile(this, asset)
                        {
                            Path = asset.Name,
                            Image = figureImage,
                        };
                        if ((assetType & AssetTypes.Shown) != AssetTypes.None)
                        {
                            Items.Add(assetType, item);
                        }
                        else
                        {
                            Items.Add(AssetTypes.Other, item);
                        }
                    }
                    else if ((assetType & AssetTypes.NotProcessed) != AssetTypes.None)
                    {

                    }
                    else 
                    {
                        Output.Write(asset.ContentType + " : " + asset.Name, Brushes.Red);
                    }
                }
            }
            if ((Generations ^ Generation.Unknown) != Generation.None)
            {
                Generations ^= Generation.Unknown;
            }

            if ((AssetTypes ^ AssetTypes.Unknown) != AssetTypes.None)
            {
                AssetTypes ^= AssetTypes.Unknown;
            }
        }
        public InstalledPackage() { }

        public InstallManifestFile InstalledManifest { get; set; }

        public string ProductName { get { return InstalledManifest.ProductName; }}
        public string InstalledLocation { get { return InstalledManifest.UserInstallPath; } }
        //public List<AssetMetadata> Assets { get; set; } = new List<AssetMetadata>(); // File in this package.
        public List<string> Files { get { return InstalledManifest.Files; } }
        public bool Selected { get => selected; set { selected = value; OnPropertyChanged(); } }
        public AssetTypes AssetTypes { get; set; } = AssetTypes.Unknown;
        public Generation Generations { get; set; } = Generation.Unknown;

        public List<InstalledFile> Characters { get; set; } = new List<InstalledFile>();
        public List<InstalledFile> Poses { get; set; } = new List<InstalledFile>();
        public List<InstalledFile> Clothings { get; set; } = new List<InstalledFile>();
        public List<InstalledFile> Others { get; set; } = new List<InstalledFile>();

        public MultiValueDictionary<AssetTypes, InstalledFile> Items { get; set; } = new MultiValueDictionary<AssetTypes, InstalledFile>();

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
