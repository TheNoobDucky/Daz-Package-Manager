using System;
using System.Collections.Generic;
using System.Text;
using Helpers;
using System.Windows.Media;
using System.IO;
using DazPackage;
using System.Linq;
using System.Windows.Data;
using System.Text.Json;
using System.Windows;

namespace Daz_Package_Manager
{
    class ProcessModel
    {
        private InstallManifestArchive archive = new InstallManifestArchive();

        public InstallManifestArchive Archive
        {
            get => archive;
            set
            {
                archive = value;
                PackagesViewSource.Source = archive.Packages;
                CharactersViewSource.Source = archive.Characters;
                PosesViewSource.Source = archive.Poses;
            }
        }

        public CollectionViewSource PackagesViewSource { get; set; } = new CollectionViewSource();
        public CollectionViewSource CharactersViewSource { get; set; } = new CollectionViewSource();
        public CollectionViewSource PosesViewSource { get; set; } = new CollectionViewSource();

        public static InstallManifestArchive Scan ()
        {
            var folder = Properties.Settings.Default.InstallManifestFolder;
            Output.Write("Start processing install archive folder: " + folder, Brushes.Gray, 0.0);
            return InstallManifestArchive.Scan(folder);
        }

        public void UnselectAll ()
        {
            Archive.Packages.ForEach(x => x.Selected = false);
        }

        public void SaveCache(string savePath)
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            File.WriteAllText(SaveFileLocation(savePath), JsonSerializer.Serialize(Archive, option));
        }

        public void LoadCache(string savePath)
        {
            try
            {
                using var packageJsonFile = File.OpenText(SaveFileLocation(savePath));
                Archive = JsonSerializer.Deserialize<InstallManifestArchive>(packageJsonFile.ReadToEnd());
            }
            catch (FileNotFoundException)
            {
            }
        }
        private string SaveFileLocation(string savePath)
        {
            return Path.Combine(Properties.Settings.Default.CacheLocation, "Archive.json");
        }

        public void SelectFigureBasedOnScene()
        {
            var sceneLocation = new FileInfo(Properties.Settings.Default.SceneFile);
            var packagesInScene = SceneFile.PackageInScene(sceneLocation, Archive.Packages);
            packagesInScene.ToList().ForEach(x => x.Selected = true);
        }

        public void GenerateVirtualInstallFolder(string destination)
        {
            var packagesToSave = Archive.Packages.Where(x => x.Selected);

            foreach (var package in packagesToSave)
            {
                var basePath = package.InstalledLocation;
                Output.Write("Installing: " + package.ProductName);
                foreach (var file in package.Files)
                {
                    var sourcePath = Path.GetFullPath(Path.Combine(basePath, file));
                    var destinationPath = Path.GetFullPath(Path.Combine(destination, file));
                    Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);
                    var errorCode = SymLinker.CreateSymlink(sourcePath, destinationPath, SymLinker.SymbolicLink.File);
                    if (errorCode != 0)
                    {
                        var error = SymLinker.DecodeErrorCode(errorCode);
                        MessageBox.Show("Failed to create symlink. Aborting. Win32 Error message:" + error);
                        return;
                    }
                }
            }
        }

    }
}
