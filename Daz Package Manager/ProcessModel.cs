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
                CharactersViewSource.Source = packages.SelectMany(x => x.Characters);
                PosesViewSource.Source = packages.SelectMany(x => x.Poses);
            }
        }

        public CollectionViewSource PackagesViewSource { get; set; } = new CollectionViewSource();

        public CollectionViewSource CharactersViewSource { get; set; } = new CollectionViewSource();
        public CollectionViewSource PosesViewSource { get; set; } = new CollectionViewSource();

        public ProcessModel()
        {
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;

            CharactersViewSource.Filter += (sender, args) =>
            {
                if (args.Item is InstalledCharacter item)
                {
                    args.Accepted = ((item.Generations & showingGeneration) != Generation.None) && ((item.Genders & showingGender) != Gender.None);
                }
            };

            PosesViewSource.Filter += (sender, args) =>
            {
                if (args.Item is InstalledPose item)
                {
                    args.Accepted = ((item.Generations & showingGeneration) != Generation.None) && ((item.Genders & showingGender) != Gender.None);
                }
            };
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            var folder = Properties.Settings.Default.InstallManifestFolder;
            Output.Write("Start processing install archive folder: " + folder, Brushes.Gray, 0.0);

            var Packages = new ConcurrentBag<InstalledPackage>();
            var files = Directory.EnumerateFiles(folder);
            Parallel.ForEach(files, x => Packages.Add(ProcessPackage(x)));

            e.Result = Packages.ToList();
        }

        private static InstalledPackage ProcessPackage(string path)
        {
            var package = new InstalledPackage(new FileInfo(path));
            Output.Write("Processed:" + package.ProductName, Brushes.Gray);
            return package;
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Packages = (List<InstalledPackage>)e.Result;
            SaveCache(Properties.Settings.Default.CacheLocation);
            Output.Write("Finished scaning install archive folder.", Brushes.Blue);
            Working = false;
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
                RefreshDisplay();
                OnPropertyChanged();
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
                RefreshDisplay();
                OnPropertyChanged();

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

        private void RefreshDisplay()
        {
            CharactersViewSource.View.Refresh();
            PosesViewSource.View.Refresh();
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public void Scan()
        {
            if (Working = !Working)
            {
                Helpers.Output.Write("Start processing.", Brushes.Green, 0.0);
                Packages.Clear();
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

        public void SelectFigureBasedOnScene()
        {
            var sceneLocation = new FileInfo(Properties.Settings.Default.SceneFile);
            var packagesInScene = SceneFile.PackageInScene(sceneLocation, Packages);
            packagesInScene.ToList().ForEach(x => x.Selected = true);
        }

        public void GenerateVirtualInstallFolder(string destination)
        {
            var packagesToSave = Packages.Where(x => x.Selected);

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
                        MessageBox.Show("Failed to create symlink. Aborting. Win32 Error message:" + error);
                        return;
                    }
                }
            }
            Output.Write("Install to virtual folder complete.");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
