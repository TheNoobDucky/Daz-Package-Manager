using DazPackage;
using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Daz_Package_Manager_Lib
{
    public class Model
    {
        public Task<List<InstalledPackage>> ScanInBackground(string folder, CancellationToken token)
        {
            var totalTime = new Stopwatch();
            totalTime.Start();
            List<InstalledPackage> packages = null;

            if (folder is null or "")
            {
                Output.Write("Please select install archive folder location", Output.Level.Error);
                return Task.FromResult(packages);
            }
            packages = new List<InstalledPackage>();

            Output.Write("Start processing install archive folder: " + folder, Output.Level.Status);
            var files = Directory.EnumerateFiles(folder).ToList();

            var totalFiles = files.Count;
            var batchSize = 200;
            var processedFiles = 0;
            //var packages = new ConcurrentBag<InstalledPackage>();
            Output.Write("Processing " + totalFiles + " files.", Output.Level.Status);

            var timer = new Stopwatch();
            timer.Start();

            for (var start = 0; start < totalFiles; start = processedFiles)
            {
                token.ThrowIfCancellationRequested();

                var numberOfFilesToProcess = Math.Min(start + batchSize, totalFiles) - start;
                
                packages.AddRange(ProcessBundle(files.GetRange(start, numberOfFilesToProcess)));

                processedFiles += numberOfFilesToProcess;

                if (timer.Elapsed.TotalSeconds > 1)
                {
                    Output.Write($"{processedFiles} / {totalFiles} files processed:", Output.Level.Alert);

                    timer.Restart();
                }
            }
            Debug.Assert(processedFiles == totalFiles, "Batch processing implemented incorrectly, missed some packages.");
            Output.Write("Total runtime: " + totalTime.Elapsed.TotalSeconds.ToString(), Output.Level.Debug);
            return Task.FromResult(packages);
        }

        private static List<InstalledPackage> ProcessBundle(List<string> files)
        {
            var packages = new ConcurrentBag<InstalledPackage>();

            Parallel.ForEach(files, file =>
            {
                try
                {
                    packages.Add(new InstalledPackage(file));
                }
                catch (DirectoryNotFoundException)
                {
                    Output.Write("Missing files for package: " + file, Output.Level.Error);
                }
                catch (CorruptFileException error)
                {
                    Output.Write(error.Message, Output.Level.Error);
                }
            });
            return packages.ToList();
        }

        public static void UnselectPackages(List<InstalledPackage> packages)
        {
            packages.ForEach(x => x.Selected = false);
        }

        public void SaveToFile(string savePath)
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            File.WriteAllText(savePath, JsonSerializer.Serialize(this, option));
        }

        public void LoadFromFile(string saveFileLocation)
        {
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
                    var model = JsonSerializer.Deserialize<Model>(packageJsonFile.ReadToEnd(), option);
                    packageJsonFile.Dispose();
                    ItemsCache = model.ItemsCache;
                    Packages = model.Packages;
                }
                catch (JsonException)
                {
                    Output.Write("Unable to load cache file. Clearing Cache.", Output.Level.Warning);
                    packageJsonFile.Dispose();
                    File.Delete(saveFileLocation);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        #region Cache
        public AssetCache ItemsCache = new AssetCache();
        private List<InstalledPackage> packages = new List<InstalledPackage>();
        public List<InstalledPackage> Packages
        {
            get => packages;
            set
            {
                packages = value;
                RebuildCache();
                OnPropertyChanged();
            }
        }

        public void RebuildCache()
        {
            ItemsCache.Clear();

            foreach (var item in packages.SelectMany(x => x.Items))
            {
                ItemsCache.AddAsset(item, item.AssetType, item.Generations, item.Genders);
            }
            foreach (var item in packages.SelectMany(x => x.OtherItems))
            {
                ItemsCache.AddAsset(item, AssetTypes.Other, item.Generations, item.Genders);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
