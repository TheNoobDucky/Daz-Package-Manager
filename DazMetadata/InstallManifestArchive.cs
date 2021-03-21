using System.Collections.Generic;
using Helpers;
using System.IO;
using System.ComponentModel;

using System.Windows.Media;

namespace DazPackage
{
    public class InstallManifestArchive
    {
        public List<InstalledPackage> Packages { get; set; } = new List<InstalledPackage>();
        public List<InstalledCharacter> Characters { get; set; } = new List<InstalledCharacter>();
        public List<InstalledPose> Poses { get; set; } = new List<InstalledPose>();
        public void AddPackage(string file)
        {
                var package = new InstalledPackage(new FileInfo(file));
                Packages.Add(package);

                Output.Write("Processing:" + package.ProductName, Brushes.Gray);

                foreach (var asset in package.Assets)
                {
                    if (InstalledCharacter.ContentTypeMatches(asset.ContentType))
                    {
                        var figureLocation = Path.Combine(package.InstalledLocation, asset.Name);
                        var figureImage = FindImage(figureLocation);
                        Characters.Add(new InstalledCharacter(package, asset.Compatibilities)
                        {
                            Path = asset.Name,
                            Image = figureImage,
                        });
                        //Output.Write("Character found: " + asset.Name);
                    }
                    else if (InstalledPose.ContentTypeMatches(asset.ContentType))
                    {
                        var figureLocation = Path.Combine(package.InstalledLocation, asset.Name);
                        var figureImage = FindImage(figureLocation);
                        Poses.Add(new InstalledPose(package, asset.Compatibilities)
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
        public void AddRange(InstallManifestArchive archive)
        {
            Packages.AddRange(archive.Packages);
            Characters.AddRange(archive.Characters);
            Poses.AddRange(archive.Poses);
        }

        private static string FindImage(string assetPath)
        {
            var figureImage = Path.ChangeExtension(assetPath, ".tip.png");
            if (!File.Exists(figureImage))
            {
                figureImage = Path.ChangeExtension(assetPath, ".png");
            }

            return figureImage;
        }
    }
}
