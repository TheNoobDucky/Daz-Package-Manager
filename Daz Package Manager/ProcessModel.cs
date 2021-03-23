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
using OsHelper;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;

namespace Daz_Package_Manager
{
    class ProcessModel : INotifyPropertyChanged
    {
        private List<InstalledPackage> packages = new List<InstalledPackage>();
        public List<InstalledPackage> Packages
        {
            get => packages;
            set
            {
                packages = value;
                PackagesViewSource.Source = packages;
                var a = packages.SelectMany(x =>
                { 
                    var y = x.Items.GetValues(AssetTypes.Clothing, false);
                    return y ?? new HashSet<InstalledFile>();
                }
                );
                Accessories.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Accessory, true));
                Attachments.Source = packages.SelectMany(x =>  x.Items.GetValues(AssetTypes.Attachment, true));
                Characters.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Character, true));
                Clothings.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Clothing, true));
                Hairs.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Hair, true));
                Morphs.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Morph, true));
                Props.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Prop, true));
                Poses.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Pose, true));
                Others.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.Other, true));
                TODO.Source = packages.SelectMany(x => x.Items.GetValues(AssetTypes.TODO, true));
            }
        }

        public CollectionViewSource PackagesViewSource { get; set; } = new CollectionViewSource();
        public CollectionViewSource Accessories { get; set; } = new CollectionViewSource();
        public CollectionViewSource Attachments { get; set; } = new CollectionViewSource();
        public CollectionViewSource Characters { get; set; } = new CollectionViewSource();
        public CollectionViewSource Clothings { get; set; } = new CollectionViewSource();
        public CollectionViewSource Hairs { get; set; } = new CollectionViewSource();
        public CollectionViewSource Morphs { get; set; } = new CollectionViewSource();
        public CollectionViewSource Props { get; set; } = new CollectionViewSource();
        public CollectionViewSource Poses { get; set; } = new CollectionViewSource();
        public CollectionViewSource Others { get; set; } = new CollectionViewSource();
        public CollectionViewSource TODO { get; set; } = new CollectionViewSource();

        public ProcessModel()
        {
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
            void FilterGenerationAndGender(object sender, FilterEventArgs args)
            {
                if (args.Item is InstalledFile item)
                {
                    args.Accepted = ((item.Generations & showingGeneration) != Generation.None) && ((item.Genders & showingGender) != Gender.None);
                }
            }
            PackagesViewSource.Filter += (sender,args) => {
                if (args.Item is InstalledPackage item)
                {
                    args.Accepted = ((item.Generations & showingGeneration) != Generation.None);
                }
            };

            Accessories.Filter += FilterGenerationAndGender;
            Attachments.Filter += FilterGenerationAndGender;
            Characters.Filter += FilterGenerationAndGender;
            Clothings.Filter += FilterGenerationAndGender;
            Hairs.Filter += FilterGenerationAndGender;
            Morphs.Filter += FilterGenerationAndGender;
            Poses.Filter += FilterGenerationAndGender;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var totalTime = new Stopwatch();
            totalTime.Start();
            BackgroundWorker worker = sender as BackgroundWorker;
            var folder = Properties.Settings.Default.InstallManifestFolder;
            if (folder is null or "")
            {
                Output.Write("Please set Install Archive folder location", Brushes.Red, 0.0);
                return;
            }

            Output.Write("Start processing install archive folder: " + folder, Brushes.Green, 0.0);
            var files = Directory.EnumerateFiles(folder).ToList();

            var numberOfFiles = files.Count;
            var batchSize = 200;
            var end = 0;
            var sanityCheck = 0;
            var wip = new ConcurrentBag<InstalledPackage>();

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
                        wip.Add(new InstalledPackage(new FileInfo(files[x])));
                    } 
                    catch (DirectoryNotFoundException)
                    {
                        Output.Write("Missing files for package: " + files[x], Brushes.Red);
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
            totalTime.Stop();
            Output.Write(totalTime.Elapsed.TotalSeconds.ToString());
            e.Result = wip.ToList();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is List<InstalledPackage> result)
            {
                Packages = result;
                SaveCache(Properties.Settings.Default.CacheLocation);
                Output.Write("Finished scaning install archive folder.", Brushes.Blue);
            }

            Working = false;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Output.Write(e.ProgressPercentage.ToString() + "% of work completed:", Brushes.Blue);
        }

        private bool working = false;

        public bool Working
        {
            get => working; private set
            {
                working = value;
                OnPropertyChanged();
            }
        }

        private double imageSize = Properties.Settings.Default.ImageSize;
        public double ImageSize
        {
            get => imageSize;
            set
            {
                Properties.Settings.Default.ImageSize = value;
                Properties.Settings.Default.Save();
                imageSize = Properties.Settings.Default.ImageSize;
                OnPropertyChanged();
            }
        }

        private bool imageVisible = true;
        public bool ImageVisible
        {
            get => imageVisible;
            set
            {
                imageVisible = value;
                OnPropertyChanged(); 
            }
        }

        #region Select Generation
        private Generation showingGeneration = Generation.All;
        public Generation ToggleGeneration
        {
            get => showingGeneration;
            set
            {
                showingGeneration ^= value;
            }
        }

        public bool ToggleGen0
        {
            get
            {
                return ToggleGeneration.HasFlag(Generation.Unknown);
            }
            set
            {
                ToggleGeneration = Generation.Unknown;
            }
        }

        public bool ToggleGen4
        {
            get
            {
                return ToggleGeneration.HasFlag(Generation.Gen4);
            }
            set
            {
                ToggleGeneration = Generation.Gen4;
            }
        }

        public bool ToggleGen5
        {
            get
            {
                return ToggleGeneration.HasFlag(Generation.Genesis_1);
            }
            set
            {
                ToggleGeneration = Generation.Genesis_1;
            }
        }

        public bool ToggleGen6
        {
            get
            {
                return ToggleGeneration.HasFlag(Generation.Genesis_2);
            }
            set
            {
                ToggleGeneration = Generation.Genesis_2;
            }
        }

        public bool ToggleGen7
        {
            get
            {
                return ToggleGeneration.HasFlag(Generation.Genesis_3);
            }
            set
            {
                ToggleGeneration = Generation.Genesis_3;
            }
        }

        public bool ToggleGen8
        {
            get
            {
                return ToggleGeneration.HasFlag(Generation.Genesis_8);
            }
            set
            {
                ToggleGeneration = Generation.Genesis_8;
            }
        }
        #endregion

        #region Select Gender
        private Gender showingGender = Gender.All;
        public Gender ToggleGender
        {
            get => showingGender;
            set
            {
                showingGender ^= value;
            }
        }

        public bool ToggleMale
        {
            get
            {
                return ToggleGender.HasFlag(Gender.Male);
            }
            set
            {
                ToggleGender = Gender.Male;
            }
        }

        public bool ToggleFemale
        {
            get
            {
                return ToggleGender.HasFlag(Gender.Female);
            }
            set
            {
                ToggleGender = Gender.Female;
            }
        }

        public bool ToggleUnknownGender
        {
            get
            {
                return ToggleGender.HasFlag(Gender.Unknown);
            }
            set
            {
                ToggleGender = Gender.Unknown;
            }
        }
        #endregion

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public void Scan()
        {
            if (Working = !Working)
            {
                Helpers.Output.Write("Start processing.", Brushes.Green, 0.0);
                worker.RunWorkerAsync();
            }
        }

        public void Cancel()
        {
            this.worker.CancelAsync();
        }

        public void UnselectAll()
        {
            Packages.ForEach(x => x.Selected = false);
        }

        public void SaveCache(string savePath)
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            File.WriteAllText(SaveFileLocation(savePath), JsonSerializer.Serialize(Packages, option));
        }

        public void LoadCache(string savePath)
        {
            try
            {
                var option = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                var saveFileLocation = SaveFileLocation(savePath);
                using var packageJsonFile = File.OpenText(saveFileLocation);
                try
                {
                    Packages = JsonSerializer.Deserialize<List<InstalledPackage>>(packageJsonFile.ReadToEnd(), option);
                }
                catch (JsonException)
                {
                    Output.Write("Unable to load cache file. Clearing Cache.");
                    packageJsonFile.Dispose();
                    File.Delete(saveFileLocation);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }
        private string SaveFileLocation(string savePath)
        {
            return Path.Combine(Properties.Settings.Default.CacheLocation, "Archive.json");
        }


        public void SelectPackageBasedOnFolder (string location)
        {
            var folder = Path.GetDirectoryName(location);
            var files = Directory.GetFiles(folder).Where(file => Path.GetExtension(file) == ".duf");
                
            foreach (var file in files)
            {
                SelectPackageBasedOnScene(file);
            }
        }

        public void SelectPackageBasedOnScene(string sceneLocation)
        {
            try
            {
                var sceneFileInfo = new FileInfo(sceneLocation);
                var packagesInScene = SceneFile.PackageInScene(sceneFileInfo, Packages);
                Output.Write("Packages Selected:", Brushes.Green);
                packagesInScene.ToList().ForEach(package=> 
                {
                    package.Selected = true;
                    Output.Write(package.ProductName, Brushes.Gray);
                });

            } catch (CorruptFileException error)
            {
                Output.Write("Invalid scene file: " + error.Message, Brushes.Red);
            } catch (ArgumentException)
            {
                Output.Write("Please select scene file.", Brushes.Red);
            }
        }



        public void GenerateVirtualInstallFolder(string destination)
        {
            var packagesToSave = Packages.Where(x => x.Selected);

            if (destination == null || destination == "")
            {
                Output.Write("Please select a location to install virtual packages to.", Brushes.Red);
                return;
            }

            Output.Write("Installing to virtual folder location: " + destination, Brushes.Green);
            foreach (var package in packagesToSave)
            {
                var basePath = package.InstalledLocation;
                Output.Write("Installing: " + package.ProductName, Brushes.Gray);
                foreach (var file in package.Files)
                {
                    var sourcePath = Path.GetFullPath(Path.Combine(basePath, file));
                    var destinationPath = Path.GetFullPath(Path.Combine(destination, file));
                    Directory.CreateDirectory(Directory.GetParent(destinationPath).FullName);
                    var errorCode = SymLinker.CreateSymlink(sourcePath, destinationPath, SymLinker.SymbolicLink.File);
                    if (errorCode != 0)
                    {
                        var error = SymLinker.DecodeErrorCode(errorCode);
                        MessageBox.Show("Aborting: \n" +
                            "Failed to create symlink, please check developer mode is turned on or run as administrator.\n" +
                            "Win32 Error Message: " + error);
                        return;
                    }
                }
            }
            Output.Write("Install to virtual folder complete.", Brushes.Green);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
