using DazPackage;
using Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Daz_Package_Manager
{
    internal class ManifestScanner
    {
        private readonly Backend model;
        private const string archiveJsonFile = "Archive.json";

        public ManifestScanner(Backend model)
        {
            this.model = model;
        }

        public async Task Scan()
        {
            tokenSource = new CancellationTokenSource();
            try
            {
                InfoBox.Write("Start processing.", InfoBox.Level.Status, 0.0);
                var sourceFolder = Properties.Settings.Default.InstallManifestFolder;
                await Task.Run(() => model.Packages.ScanInBackground(sourceFolder, tokenSource.Token), tokenSource.Token);
                InfoBox.Write($"Finished scanning install manifest folder task finished.", InfoBox.Level.Status);
            }
            catch (TargetInvocationException error)
            {
                InfoBox.Write($"Error source: {error.InnerException.Source}", InfoBox.Level.Error);
                InfoBox.Write($"Error error message: {error.InnerException.Message}", InfoBox.Level.Error);
            }
            catch (OperationCanceledException)
            {
                InfoBox.Write($"Manifest scan task canceled.", InfoBox.Level.Status);
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        public void SaveCache()
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var savePath = CacheManager.SaveFileLocation(archiveJsonFile);
            File.WriteAllText(savePath, JsonSerializer.Serialize(model.Packages.Packages, option));
        }

        public async Task LoadCache()
        {
            try
            {
                tokenSource = new();
                await Task.Run(() => LoadCache_Imple(), tokenSource.Token);
            }
            catch (TargetInvocationException error)
            {
                InfoBox.Write($"Error source: {error.InnerException.Source}", InfoBox.Level.Error);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        private void LoadCache_Imple ()
        {
            var saveFileLocation = CacheManager.SaveFileLocation(archiveJsonFile);
            try
            {
                var option = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                using var packageJsonFile = File.OpenText(saveFileLocation);
                try
                {
                    var packagesCache = JsonSerializer.Deserialize<List<InstalledPackage>>(packageJsonFile.ReadToEnd(), option);
                    packageJsonFile.Dispose();
                    model.Packages.Packages = packagesCache;
                }
                catch (JsonException)
                {
                    InfoBox.Write("Unable to load cache file. Clearing Cache.", InfoBox.Level.Warning);
                    packageJsonFile.Dispose();
                    File.Delete(saveFileLocation);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        private CancellationTokenSource tokenSource = null;
        public void Cancel()
        {
            InfoBox.Write("Canceling manifest scan task.", InfoBox.Level.Status);
            tokenSource.Cancel();
        }
    }
}
