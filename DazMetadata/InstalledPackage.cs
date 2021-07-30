using Output;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DazPackage
{
    /// <summary>
    /// Represent a zip package installed (ie have an install manifest).
    /// </summary>
    public class InstalledPackage : INotifyPropertyChanged
    {
        public string ProductName { get; set; }
        public string InstalledLocation { get; set; }
        //public List<AssetMetadata> Assets { get; set; } = new List<AssetMetadata>(); // File in this package.
        public List<string> Files { get; set; }
        public bool Selected { get => selected; set { selected = value; OnPropertyChanged(); } }
        public AssetTypes AssetTypes { get; set; } = AssetTypes.Unknown;
        public Generation Generations { get; set; } = Generation.Unknown;
        public Gender Genders { get; set; } = Gender.Unknown;

        public List<InstalledFile> Items { get; set; } = new List<InstalledFile>();
        public List<InstalledFile> OtherItems { get; set; } = new List<InstalledFile>();

        public InstalledPackage(string file)
        {
            var fileInfo = new FileInfo(file);
            var installManifest = new InstallManifestFile(fileInfo);
            ProductName = installManifest.ProductName;
            InstalledLocation = installManifest.UserInstallPath;
            Files = installManifest.Files;

            foreach (var metadataFileLocation in installManifest.MetadataFiles)
            {
                var metadataFilePath = new FileInfo(Path.Combine(installManifest.UserInstallPath, metadataFileLocation));
                try
                {
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
                                Items.Add(item);
                            }
                            else
                            {
                                OtherItems.Add(item);
                            }
                            Generations |= item.Generations;
                            Genders |= item.Genders;
                            AssetTypes |= assetType;
                        }
                        else if ((assetType & AssetTypes.NotProcessed) != AssetTypes.None)
                        {

                        }
                        else
                        {
                            InfoBox.Write($"{asset.ContentType} : {asset.Name}", InfoBox.Level.Debug);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    InfoBox.Write($"Missing metadatafile: {metadataFilePath}", InfoBox.Level.Error);
                }
            }
            if ((Generations ^ Generation.Unknown) != Generation.None)
            {
                Generations ^= Generation.Unknown;
            }

            if ((Genders ^ Gender.Unknown) != Gender.None)
            {
                Genders ^= Gender.Unknown;
            }

            if ((AssetTypes ^ AssetTypes.Unknown) != AssetTypes.None)
            {
                AssetTypes ^= AssetTypes.Unknown;
            }
        }
        public InstalledPackage() { }

        private bool selected = false;
        private static string FindImage(string assetPath)
        {
            // Note: this is faster than GetFiles
            // Largest image used in hover over menu.
            var figureImage = Path.ChangeExtension(assetPath, ".tip.png");

            if (File.Exists(figureImage))
            {
                return figureImage;
            }

            // Normal image.
            figureImage = Path.ChangeExtension(assetPath, ".png");
            if (File.Exists(figureImage))
            {
                return figureImage;
            }

            // Sometimes it is named this way.
            figureImage = Path.ChangeExtension(assetPath, ".duf.png");
            return File.Exists(figureImage) ? figureImage : "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
