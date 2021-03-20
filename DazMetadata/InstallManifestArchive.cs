using System.Collections.Generic;
using Helpers;
using System.IO;
using System.Linq;

using System.Windows.Media;

namespace DazPackage
{
    public class InstallManifestArchive
    {
        public List<InstalledPackage> Packages { get; set; } = new List<InstalledPackage>();
        public List<InstalledCharacter> Characters { get; set; } = new List<InstalledCharacter>();
        public List<InstalledPose> Poses { get; set; } = new List<InstalledPose>();
        public static InstallManifestArchive Scan(string folder)
        {
            var archive = new InstallManifestArchive();
            var files = Directory.EnumerateFiles(folder);

            foreach (var file in files)
            {
                var package = new InstalledPackage(new FileInfo(file));
                archive.Packages.Add(package);

                Output.Write("Processing:" + package.ProductName, Brushes.Gray);

                foreach (var asset in package.Assets)
                {
                    if (InstalledCharacter.ContentTypeMatches(asset.ContentType))
                    {
                        var figureLocation = Path.Combine(package.InstalledLocation, asset.Name);
                        var figureImage = FindImage(figureLocation);
                        archive.Characters.Add(new InstalledCharacter(package)
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
                        archive.Poses.Add(new InstalledPose(package)
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
            return archive;
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
