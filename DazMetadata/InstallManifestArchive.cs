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

            archive.Packages = files.Select(x => new InstalledPackage(new FileInfo(x))).ToList();

            (archive.Characters, archive.Poses) = GenerateItemLists(archive.Packages);

            return archive;
        }

        public static (List<InstalledCharacter> figures, List<InstalledPose> poses) GenerateItemLists(IEnumerable<InstalledPackage> installedPackages)
        {
            var figures = new List<InstalledCharacter>();
            var poses = new List<InstalledPose>();

            foreach (var package in installedPackages)
            {
                foreach (var asset in package.Assets)
                {
                    if (InstalledCharacter.ContentTypeMatches(asset.ContentType))
                    {
                        var figureLocation = Path.Combine(package.InstalledLocation, asset.Name);
                        var figureImage = FindImage(figureLocation);
                        figures.Add(new InstalledCharacter(package)
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
                        poses.Add(new InstalledPose(package)
                        {
                            Path = asset.Name,
                            Image = figureImage,
                        });
                        //Output.Write("Pose found: " + asset.Name);
                    }
                    else if (InstalledMaterial.ContentTypeMatches(asset.ContentType))
                    {

                    }
                    else
                    {
                        Output.Write(asset.ContentType, Brushes.Red);
                    }
                }
            }
            return (figures, poses);
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
