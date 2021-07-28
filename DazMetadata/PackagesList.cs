using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DazPackage
{
    public class PackagesList : INotifyPropertyChanged
    {
        public Task ScanInBackground(string folder, CancellationToken token)
        {
            var totalTime = new Stopwatch();
            totalTime.Start();
            List<InstalledPackage> newPackages = null;

            if (folder is null or "")
            {
                Output.Write("Please select install archive folder location", Output.Level.Error);
                return Task.FromResult(newPackages);
            }
            newPackages = new List<InstalledPackage>();

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

                newPackages.AddRange(ProcessBundle(files.GetRange(start, numberOfFilesToProcess)));

                processedFiles += numberOfFilesToProcess;

                if (timer.Elapsed.TotalSeconds > 1)
                {
                    Output.Write($"{processedFiles} / {totalFiles} files processed:", Output.Level.Alert);

                    timer.Restart();
                }
            }
            Debug.Assert(processedFiles == totalFiles, "Batch processing implemented incorrectly, missed some packages.");
            Output.Write("Total runtime: " + totalTime.Elapsed.TotalSeconds.ToString(), Output.Level.Debug);
            Packages = newPackages;
            return Task.CompletedTask;
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

        #region Cache
        public AssetCache ItemsCache = new();
        private List<InstalledPackage> packages = new();
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
            Application.Current.Dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
        }
        #endregion
    }
}
