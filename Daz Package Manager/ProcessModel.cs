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
            PackagesViewSource.Source = packageModel.Packages.Where(x => x.Generations.CheckFlag(showingGeneration) && x.Genders.CheckFlag(showingGender));
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
            PackagesViewSource.GroupDescriptions.Add(generationGrouping);
            PackagesViewSource.GroupDescriptions.Add(genderGrouping);
            PackagesViewSource.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Accessories.GroupDescriptions.Add(generationGrouping);
            Accessories.GroupDescriptions.Add(genderGrouping);
            Accessories.GroupDescriptions.Add(itemCategoriesGrouping);
            Accessories.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Attachments.GroupDescriptions.Add(generationGrouping);
            Attachments.GroupDescriptions.Add(genderGrouping);
            Attachments.GroupDescriptions.Add(itemCategoriesGrouping);
            Attachments.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Characters.GroupDescriptions.Add(generationGrouping);
            Characters.GroupDescriptions.Add(genderGrouping);
            Characters.GroupDescriptions.Add(itemCategoriesGrouping);
            Characters.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Clothings.GroupDescriptions.Add(generationGrouping);
            Clothings.GroupDescriptions.Add(genderGrouping);
            Clothings.GroupDescriptions.Add(itemCategoriesGrouping);
            Clothings.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Hairs.GroupDescriptions.Add(generationGrouping);
            Hairs.GroupDescriptions.Add(genderGrouping);
            Hairs.GroupDescriptions.Add(itemCategoriesGrouping);
            Hairs.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Morphs.GroupDescriptions.Add(generationGrouping);
            Morphs.GroupDescriptions.Add(genderGrouping);
            Morphs.GroupDescriptions.Add(itemCategoriesGrouping);
            Morphs.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Others.GroupDescriptions.Add(itemContentGrouping);
            Others.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Props.GroupDescriptions.Add(itemCategoriesGrouping);
            Props.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Poses.GroupDescriptions.Add(generationGrouping);
            Poses.GroupDescriptions.Add(genderGrouping);
            Poses.GroupDescriptions.Add(itemCategoriesGrouping);
            Poses.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            TODO.GroupDescriptions.Add(itemContentGrouping);
            TODO.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

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

        public void SelectPackagesBasedOnFolder(string location)
        {
            var folder = Path.GetDirectoryName(location);
            var files = Directory.GetFiles(folder).Where(file => Path.GetExtension(file) == ".duf");

            foreach (var file in files)
            {
                SelectPackagesBasedOnScene(file);
            }
        }

        public void SelectPackagesBasedOnScene(string sceneLocation)
        {
            try
            {
                var sceneFileInfo = new FileInfo(sceneLocation);
                var (packagesInScene, remainingFiles) = SceneFile.PackagesInScene(sceneFileInfo, Packages);
                Output.Write("Packages Selected:", Output.Level.Status);
                packagesInScene.ForEach(package =>
                {
                    package.Selected = true;
                    Output.Write(package.ProductName, Output.Level.Info);
                });
                if (remainingFiles.Count > 0)
                {

                    Output.Write("Unable to find reference for the following files:", Output.Level.Warning);
                    remainingFiles.ForEach(file => Output.Write(file, Output.Level.Info));
                }
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

        public void GenerateVirtualInstallFolder(string destination, bool makeCopy = false, bool warnMissingFile = false)
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
                    VirtualPackage.Install(package, destination, makeCopy, warnMissingFile);
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

        private static readonly GenerationToStringConverter generationToStringConverter = new GenerationToStringConverter();
        private static readonly GenerationGroupCompare generationGroupCompare = new GenerationGroupCompare();
        private static readonly PropertyGroupDescription generationGrouping = new PropertyGroupDescription("Generations", generationToStringConverter)
        {
            CustomSort = generationGroupCompare
        };

        private static readonly StringCompareHelper stringCompare = new StringCompareHelper();
        private static readonly AssetToStringConverter installedPackageAssetTypeConverter = new AssetToStringConverter();
        private static readonly PropertyGroupDescription itemAssetTypeGrouping = new PropertyGroupDescription("AssetTypes", installedPackageAssetTypeConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly GenderToStringConverter genderToStringConverter = new GenderToStringConverter();
        private static readonly PropertyGroupDescription genderGrouping = new PropertyGroupDescription("Genders", genderToStringConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly ContentTypeToDisplayConverter installedItemContentTypeConverter = new ContentTypeToDisplayConverter();
        private static readonly PropertyGroupDescription itemContentGrouping = new PropertyGroupDescription("ContentType", installedItemContentTypeConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly CategoriesConverter installedItemCategoriesConverter = new CategoriesConverter();
        private static readonly PropertyGroupDescription itemCategoriesGrouping = new PropertyGroupDescription("Categories", installedItemCategoriesConverter)
        {
            CustomSort = stringCompare
        };
    }
}
