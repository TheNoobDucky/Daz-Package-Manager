﻿using DazPackage;
using Helpers;
using OsHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Daz_Package_Manager
{
    internal class ModelView : INotifyPropertyChanged
    {
        public PackagesList packages = new();

        public ModelView()
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

            packages.PropertyChanged += ModelChangedHandler;
            settings.PropertyChanged += GuiSettingChangedHandler;
        }

        private CancellationTokenSource ManifestScanToken = null;

        public async Task ScanManifestFolder()
        {
            ManifestScanToken = new CancellationTokenSource();
            try
            {
                Helpers.Output.Write("Start processing.", Output.Level.Status, 0.0);
                var sourceFolder = Properties.Settings.Default.InstallManifestFolder;
                await Task.Run(() => packages.ScanInBackground(sourceFolder, ManifestScanToken.Token), ManifestScanToken.Token);
                Output.Write($"Finished scanning install manifest folder task finished.", Output.Level.Status);
            }
            catch (TargetInvocationException error)
            {
                Output.Write($"Error source: {error.InnerException.Source.ToString()}", Output.Level.Error);
                Output.Write($"Error error message: {error.InnerException.Message}", Output.Level.Error);
            }
            catch (OperationCanceledException)
            {
                Output.Write($"Manifest scan task canceled.", Output.Level.Status);
            }
            finally
            {
                ManifestScanToken.Dispose();
            }
        }

        public void CancelManifestScan()
        {
            Output.Write("Canceling manifest scan task.", Output.Level.Status);
            ManifestScanToken.Cancel();
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

        public void UnselectAll() 
        {
            PackagesList.UnselectPackages(packages.Packages);
        }

        public void SelectPackagesBasedOnScene(string sceneLocation)
        {
            try
            {
                var sceneFileInfo = new FileInfo(sceneLocation);
                var (packagesInScene, remainingFiles) = SceneFile.PackagesInScene(sceneFileInfo, packages.Packages);
                Output.Write("Packages Selected:", Output.Level.Status);
                packagesInScene.ForEach(package =>
                {
                    package.Selected = true;
                    Output.Write(package.ProductName, Output.Level.Info);
                });
                if (remainingFiles.Count > 0)
                {

                    Output.Write("Unable to find reference for the following files:", Output.Level.Status);
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

            var packagesToSave = packages.Packages.Where(x => x.Selected);

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

        #region Updating Packages
        private void ModelChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Packages")
            {
                SavePackagesCacheToFile();
                UpdateSelections();
            }
        }

        public void SavePackagesCacheToFile()
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var savePath = SaveFileLocation();
            File.WriteAllText(savePath, JsonSerializer.Serialize(packages.Packages, option));
        }

        public void LoadPackagesCache()
        {
            var saveFileLocation = SaveFileLocation();
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
                    packages.Packages = packagesCache;
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

        private static string SaveFileLocation()
        {
            return Path.Combine(Properties.Settings.Default.CacheLocation, "Archive.json");
        }
        #endregion

        #region Product Views
        public void UpdateSelections()
        {
            PackagesViewSource.Source = packages.Packages.Where(x => x.Generations.CheckFlag(settings.ToggleGeneration) && x.Genders.CheckFlag(settings.ToggleGender));
            Accessories.Source = packages.ItemsCache.GetAssets(AssetTypes.Accessory, settings.ToggleGeneration, settings.ToggleGender);
            Attachments.Source = packages.ItemsCache.GetAssets(AssetTypes.Attachment, settings.ToggleGeneration, settings.ToggleGender);
            Characters.Source = packages.ItemsCache.GetAssets(AssetTypes.Character, settings.ToggleGeneration, settings.ToggleGender);
            Clothings.Source = packages.ItemsCache.GetAssets(AssetTypes.Clothing, settings.ToggleGeneration, settings.ToggleGender);
            Hairs.Source = packages.ItemsCache.GetAssets(AssetTypes.Hair, settings.ToggleGeneration, settings.ToggleGender);
            Morphs.Source = packages.ItemsCache.GetAssets(AssetTypes.Morph, settings.ToggleGeneration, settings.ToggleGender);
            Props.Source = packages.ItemsCache.GetAssets(AssetTypes.Prop, settings.ToggleGeneration, settings.ToggleGender);
            Poses.Source = packages.ItemsCache.GetAssets(AssetTypes.Pose, settings.ToggleGeneration, settings.ToggleGender);
            Others.Source = packages.ItemsCache.GetAssets(AssetTypes.Other, settings.ToggleGeneration, settings.ToggleGender);
            TODO.Source = packages.ItemsCache.GetAssets(AssetTypes.TODO, settings.ToggleGeneration, settings.ToggleGender);
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
        #endregion

        #region Gui Settings
        private GUISettings settings = new ();
        public GUISettings Settings {
            get { return settings; }
            set
            {
                if (value != settings)
                {
                    settings = value;
                    OnPropertyChanged();
                }
            }
        }

        private void GuiSettingChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "ToggleGeneration" or "ToggleGender")
            {
                UpdateSelections();
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

    }
}
