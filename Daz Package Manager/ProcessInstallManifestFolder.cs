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

            var figures = GenerateItemLists(installedPackages);

            return (installedPackages, figures);
        }

        public static List<InstalledCharacter> GenerateItemLists (IEnumerable<InstalledPackage> installedPackages)
        {
            var figures = new List<InstalledCharacter>();
            foreach (var package in installedPackages)
            {
                foreach (var asset in package.Assets)
                {
                    if (asset.ContentType == "Actor/Character")
                    {
                        var figureLocation = Path.Combine(package.InstalledLocation,asset.Name);
                        var figureImage = Path.ChangeExtension(figureLocation, ".tip.png");
                        if (!File.Exists(figureImage))
                        {
                            figureImage = Path.ChangeExtension(figureLocation, ".png");
                        }
                        figures.Add(new InstalledCharacter(package) {
                            CharacterName = asset.Name,
                            CharacterImage = figureImage,
                        }); ;
                        Output.Write("Character found: " + asset.Name);
                    }
                }
            }
            return figures;
        }
    }
}
