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
using System.Threading.Tasks;

namespace Daz_Package_Manager
{
    public class PackageModel
    {
        public PackageModel()
        {
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
        }

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

        public void ScanInstallManifestFolderAsync(string folder)
        {
            worker.RunWorkerAsync(folder);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var totalTime = new Stopwatch();
            totalTime.Start();
            BackgroundWorker worker = sender as BackgroundWorker;

            var folder = e.Argument as string;
            if (folder is null or "")
            {
                Output.Write("Please set install archive folder location", Output.Level.Error);
                return;
            }

            Output.Write("Start processing install archive folder: " + folder, Output.Level.Status);
            var files = Directory.EnumerateFiles(folder).ToList();

            var numberOfFiles = files.Count;
            var batchSize = 200;
            var end = 0;
            var sanityCheck = 0;
            var packages = new ConcurrentBag<InstalledPackage>();
            Output.Write("Processing " + numberOfFiles + " files.", Output.Level.Status);

            var timer = new Stopwatch();
            timer.Start();

            for (var start = 0; start < numberOfFiles; start = end)
            {
                end = Math.Min(start + batchSize, numberOfFiles);
                var count = end - start;

                Parallel.For(start, end, x =>
                {
                    try
                    {
                        packages.Add(new InstalledPackage(new FileInfo(files[x])));
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Output.Write("Missing files for package: " + files[x], Output.Level.Error);
                    }
                    catch (CorruptFileException error)
                    {
                        Output.Write(error.Message, Output.Level.Error);
                    }
                });
                sanityCheck += count;

                var progress = sanityCheck * 100 / numberOfFiles;
                if (timer.Elapsed.TotalSeconds > 1)
                {
                    worker.ReportProgress(progress, null);
                    timer.Restart();
                }
            }
            Debug.Assert(sanityCheck == numberOfFiles, "Batch processing implemented incorrectly, missed some packages.");
            Output.Write("Total runtime: " + totalTime.Elapsed.TotalSeconds.ToString(), Output.Level.Debug);
            e.Result = packages.ToList();
        }

        public void RebuildCache()
        {
            foreach (var item in packages.SelectMany(x => x.Items))
            {
                ItemsCache.AddAsset(item, item.AssetType, item.Generations, item.Genders);
            }
            foreach (var item in packages.SelectMany(x => x.OtherItems))
            {
                ItemsCache.AddAsset(item, AssetTypes.Other, item.Generations, item.Genders);
            }
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Result is List<InstalledPackage> packages)
                {
                    RebuildCache();
                    Packages = packages;
                    Output.Write("Finished scaning install archive folder.", Output.Level.Status);
                }
            }
            catch (TargetInvocationException error)
            {
                Output.Write("Error source: " + error.InnerException.Source.ToString(), Output.Level.Error);
                Output.Write("Error error message: " + error.InnerException.Message, Output.Level.Error);
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Output.Write(e.ProgressPercentage.ToString() + "% of work completed:", Output.Level.Alert);
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
