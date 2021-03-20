using System;
using System.Collections.Generic;
using System.Text;
using Helpers;
using System.Windows.Media;
using System.IO;
using DazPackage;
using System.Linq;
using System.Collections.ObjectModel;

namespace Daz_Package_Manager
{
    class ProcessInstallManifestFolder
    {
        public static (List<InstalledPackage>, List<InstalledCharacter>) Scan ()
        {
            var folder = Properties.Settings.Default.InstallManifestFolder;
            Output.Write("Start processing install archive folder: " + folder, Brushes.Gray, 0.0);
            var files = Directory.EnumerateFiles(folder);

            var installedPackages = files.Select(x => new InstalledPackage(new FileInfo(x))).ToList();

            var (figures, poses) = GenerateItemLists(installedPackages);

            return (installedPackages, figures);
        }

        public static (List<InstalledCharacter> figures, List<InstalledPose> poses) GenerateItemLists (IEnumerable<InstalledPackage> installedPackages)
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
                        Output.Write("Pose found: " + asset.Name);
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


        private static string FindImage (string assetPath)
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
