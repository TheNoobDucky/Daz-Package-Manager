using DazPackage;
using Helpers;
using OsHelper;
using Output;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace Daz_Package_Manager
{
    internal class SelectPackages
    {
        private readonly Backend model;
        public SelectPackages(Backend model)
        {
            this.model = model;
        }

        public void BasedOnFolder(string location)
        {
            var folder = Path.GetDirectoryName(location);
            SelectPackagesInFolder(folder);
        }

        private void SelectPackagesInFolder(string folder)
        {
            var files = Directory.EnumerateFiles(folder).Where(file => Path.GetExtension(file) == ".duf");
            foreach (var file in files)
            {
                BasedOnScene(file);
            }

            var subfolders = Directory.EnumerateDirectories(folder);
            foreach (var subfolder in subfolders)
            {
                SelectPackagesInFolder(subfolder);
            }
        }

        public void BasedOnScene(string sceneLocation)
        {
            try
            {
                var sceneFileInfo = new FileInfo(sceneLocation);
                var (packagesInScene, remainingFiles) = DufFile.PackagesInFile(sceneFileInfo, model.Packages.Packages);
                InfoBox.Write("Packages Selected:", InfoBox.Level.Status);
                packagesInScene.ForEach(package =>
                {
                    package.Selected = true;
                    InfoBox.Write(package.ProductName, InfoBox.Level.Info);
                });

                if (remainingFiles.Count > 0)
                {
                    var (foundFiles, missingFiles) = model.ThirdParty.GetFiles(remainingFiles);
                    if (foundFiles.Any())
                    {
                        InfoBox.Write("3rd Party files Selected:", InfoBox.Level.Status);
                        foreach (var file in foundFiles)
                        {
                            file.ParentFolder.Selected = true;
                            InfoBox.Write(file.Location, InfoBox.Level.Info);
                        }
                    }
                    if (missingFiles.Count > 0)
                    {
                        InfoBox.Write("Unable to find reference for the following files:", InfoBox.Level.Status);
                        missingFiles.ForEach(file => InfoBox.Write(file, InfoBox.Level.Info));
                    }
                }
            }
            catch (CorruptFileException error)
            {
                InfoBox.Write($"Invalid scene file: {error.Message}", InfoBox.Level.Error);
            }
            catch (ArgumentException)
            {
                InfoBox.Write("Please select scene file.", InfoBox.Level.Error);
            }
        }

        private CancellationTokenSource tokenSource = null;

        public void Cancel()
        {
            InfoBox.Write("Canceling selecting packages.", InfoBox.Level.Status);
            tokenSource.Cancel();
        }
    }
}
