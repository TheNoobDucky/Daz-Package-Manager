using DazPackage;
using Helpers;
using OsHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Data;
namespace Daz_Package_Manager
{
    class ProcessModel : INotifyPropertyChanged
    {
        public PackageModel packageModel = new PackageModel();

        public List<InstalledPackage> Packages
        {
            get => packageModel.Packages;
            set
            {
                packageModel.Packages = value;
                UpdateSelections();
            }
        }

        private void ModelChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Packages")
            {
                packageModel.SaveToFile(SaveFileLocation());
                Working = false;
                UpdateSelections();
            }
            else
            {

            }
            //UpdateSelections();
        }

        public void UpdateSelections()
        {
            PackagesViewSource.Source = packageModel.Packages.Where(x => x.Generations.CheckFlag(showingGeneration));
            Accessories.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Accessory, showingGeneration, showingGender);
            Attachments.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Attachment, showingGeneration, showingGender);
            Characters.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Character, showingGeneration, showingGender);
            Clothings.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Clothing, showingGeneration, showingGender);
            Hairs.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Hair, showingGeneration, showingGender);
            Morphs.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Morph, showingGeneration, showingGender);
            Props.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Prop, showingGeneration, showingGender);
            Poses.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Pose, showingGeneration, showingGender);
            Others.Source = packageModel.ItemsCache.GetAssets(AssetTypes.Other, showingGeneration, showingGender);
            TODO.Source = packageModel.ItemsCache.GetAssets(AssetTypes.TODO, showingGeneration, showingGender);
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
            PackagesViewSource.GroupDescriptions.Add(itemAssetTypeGrouping);

            Others.GroupDescriptions.Add(itemContentGrouping);
            TODO.GroupDescriptions.Add(itemContentGrouping);

            Accessories.GroupDescriptions.Add(itemCategoriesGrouping);
            Attachments.GroupDescriptions.Add(itemCategoriesGrouping);
            Clothings.GroupDescriptions.Add(itemCategoriesGrouping);
            Morphs.GroupDescriptions.Add(itemCategoriesGrouping);
            Props.GroupDescriptions.Add(itemCategoriesGrouping);
            Hairs.GroupDescriptions.Add(itemCategoriesGrouping);
            Characters.GroupDescriptions.Add(itemCategoriesGrouping);
            Poses.GroupDescriptions.Add(itemCategoriesGrouping);

            packageModel.PropertyChanged += ModelChangedHandler;
        }

        private bool working = false;
        public bool Working
        {
            get => working;
            private set
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
            set { showingGeneration ^= value; UpdateSelections(); }
        }

        public bool ToggleGen0
        {
            get => ToggleGeneration.HasFlag(Generation.Unknown);
            set => ToggleGeneration = Generation.Unknown;
        }

        public bool ToggleGen4
        {
            get => ToggleGeneration.HasFlag(Generation.Gen4);
            set => ToggleGeneration = Generation.Gen4;
        }

        public bool ToggleGen5
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_1);
            set => ToggleGeneration = Generation.Genesis_1;
        }

        public bool ToggleGen6
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_2);
            set => ToggleGeneration = Generation.Genesis_2;
        }

        public bool ToggleGen7
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_3);
            set => ToggleGeneration = Generation.Genesis_3;
        }

        public bool ToggleGen8
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_8);
            set => ToggleGeneration = Generation.Genesis_8;
        }
        #endregion

        #region Select Gender
        private Gender showingGender = Gender.All;
        public Gender ToggleGender
        {
            get => showingGender;
            set { showingGender ^= value; UpdateSelections(); }
        }

        public bool ToggleMale
        {
            get => ToggleGender.HasFlag(Gender.Male);
            set => ToggleGender = Gender.Male;
        }

        public bool ToggleFemale
        {
            get => ToggleGender.HasFlag(Gender.Female);
            set => ToggleGender = Gender.Female;
        }

        public bool ToggleUnknownGender
        {
            get => ToggleGender.HasFlag(Gender.Unknown);
            set => ToggleGender = Gender.Unknown;
        }
        #endregion


        public void Scan()
        {
            if (Working = !Working)
            {
                Helpers.Output.Write("Start processing.", Output.Level.Status, 0.0);
                packageModel.ScanInstallManifestFolderAsync(Properties.Settings.Default.InstallManifestFolder);
            }
        }


        public void LoadCache()
        {
            packageModel.LoadFromFile(SaveFileLocation());
        }

        private static string SaveFileLocation()
        {
            return Path.Combine(Properties.Settings.Default.CacheLocation, "Archive.json");
        }

        public void SelectPackageBasedOnFolder(string location)
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
                Output.Write("Packages Selected:", Output.Level.Status);
                packagesInScene.ToList().ForEach(package =>
                {
                    package.Selected = true;
                    Output.Write(package.ProductName, Output.Level.Info);
                });

            }
            catch (CorruptFileException error)
            {
                Output.Write("Invalid scene file: " + error.Message, Output.Level.Error);
            }
            catch (ArgumentException)
            {
                Output.Write("Please select scene file.", Output.Level.Error);
            }
        }

        public void GenerateVirtualInstallFolder(string destination, bool makeCopy = false)
        {
            if (destination == null || destination == "")
            {
                Output.Write("Please select a location to install virtual packages to.", Output.Level.Error);
                return;
            }

            var packagesToSave = Packages.Where(x => x.Selected);

            Output.Write("Installing to virtual folder location: " + destination, Output.Level.Status);

            foreach (var package in packagesToSave)
            {
                Output.Write("Installing: " + package.ProductName, Output.Level.Info);
                try
                {
                    VirtualPackage.Install(package, destination, makeCopy);
                } 
                catch (SymLinkerError error)
                {
                    Output.Write("Error creating symlink file, aborting.", Output.Level.Error);
                    MessageBox.Show(error.Message);
                }
            }
            Output.Write("Install to virtual folder complete.", Output.Level.Status);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private static readonly GenerationStringConverter generationStringConverter = new GenerationStringConverter();
        private static readonly GenerationGroupCompare generationGroupCompare = new GenerationGroupCompare();
        private static readonly PropertyGroupDescription packageGroup = new PropertyGroupDescription("Generations", generationStringConverter)
        {
            CustomSort = generationGroupCompare
        };

        private static readonly StringCompareHelper itemGroupCompare = new StringCompareHelper();
        private static readonly InstalledItemContentTypeConverter installedPackageAssetTypeConverter = new InstalledItemContentTypeConverter();
        private static readonly PropertyGroupDescription itemAssetTypeGrouping = new PropertyGroupDescription("AssetTypes", installedPackageAssetTypeConverter)
        {
            CustomSort = itemGroupCompare
        };

        private static readonly InstalledItemContentTypeConverter installedItemContentTypeConverter = new InstalledItemContentTypeConverter();
        private static readonly PropertyGroupDescription itemContentGrouping = new PropertyGroupDescription("ContentType", installedItemContentTypeConverter)
        {
            CustomSort = itemGroupCompare
        };

        private static readonly InstalledItemCategoriesCnverter installedItemCategoriesConverter = new InstalledItemCategoriesCnverter();
        private static readonly PropertyGroupDescription itemCategoriesGrouping = new PropertyGroupDescription("Categories", installedItemCategoriesConverter)
        {
            CustomSort = itemGroupCompare
        };
    }
}
