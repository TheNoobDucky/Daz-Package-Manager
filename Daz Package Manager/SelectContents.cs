using DazPackage;
using Helpers;
using Output;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Daz_Package_Manager
{
    internal class SelectContents
    {
        private readonly Backend model;
        public SelectContents(Backend model)
        {
            this.model = model;
        }

        public async Task BasedOnFolder(string location)
        {
            try
            {
                tokenSource = new();
                var folder = Path.GetDirectoryName(location);
                await Task.Run(() => SelectPackagesInFolder(folder, tokenSource.Token), tokenSource.Token);
            }
            catch (TargetInvocationException error)
            {
                InfoBox.Write($"Invoke error Error source: {error.InnerException.Source}", InfoBox.Level.Error);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        private Task SelectPackagesInFolder(string folder, CancellationToken token)
        {
            if (folder is null or "") {
                InfoBox.Write("Please select a scene file.", InfoBox.Level.Error);
                return Task.CompletedTask;
            }
            try
            {
                var files = Directory.EnumerateFiles(folder).Where(file => Path.GetExtension(file) == ".duf");
                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
                    Select_Imple(file);
                }

                var subfolders = Directory.EnumerateDirectories(folder);
                foreach (var subfolder in subfolders)
                {
                    _ = SelectPackagesInFolder(subfolder, token);
                }
            } 
            catch (DirectoryNotFoundException e)
            {
                Output.InfoBox.Write($"{e.Message}", InfoBox.Level.Error);
            }

            return Task.CompletedTask;
        }

        public async Task BasedOnScene(string location)
        {
            //var folder = Path.GetDirectoryName(location);
            await Task.Run(() => Select_Imple(location));
        }

        private void Select_Imple (string sceneLocation)
        {
            if (sceneLocation is null or "")
            {
                InfoBox.Write("Please select a scene file.", InfoBox.Level.Error);
                return;
            }

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
            //catch (ArgumentException)
            //{
            //    InfoBox.Write("Please select scene file.", InfoBox.Level.Error);
            //}
        }

        private CancellationTokenSource tokenSource;

        public void Cancel()
        {
            InfoBox.Write("Canceling selecting packages.", InfoBox.Level.Status);
            tokenSource.Cancel();
        }
    }
}
