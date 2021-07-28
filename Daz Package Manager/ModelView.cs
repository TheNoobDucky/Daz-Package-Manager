using DazPackage;
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

            ThirdParty.PropertyChanged += UpdateThirdPartyView;
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
                    var (foundFiles, missingFiles) = ThirdParty.GetFiles(remainingFiles);
                    if (foundFiles.Any())
                    {
                        Output.Write("3rd Party files Selected:", Output.Level.Status);
                        foreach (var file in foundFiles)
                        {
                            file.Folder.Selected = true;
                            Output.Write(file.Path, Output.Level.Info);
                        }
                    }
                    if (missingFiles.Count > 0)
                    {
                        Output.Write("Unable to find reference for the following files:", Output.Level.Status);
                        missingFiles.ForEach(file => Output.Write(file, Output.Level.Info));
                    }
                }
            }
            catch (CorruptFileException error)
            {
                Output.Write($"Invalid scene file: {error.Message}", Output.Level.Error);
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

            Output.Write("Installing to virtual folder location: " + destination, Output.Level.Status);

            var packagesToSave = SelectedPackages();
            foreach (var package in packagesToSave)
            {
                Output.Write("Installing: " + package.ProductName, Output.Level.Info);
                try
                {
                    VirtualPackage.Install(package, destination, makeCopy, warnMissingFile);
                }
                catch (SymLinkerError error)
                {
                    Output.Write($"Unable to copy file {error.Message}", Output.Level.Error);
                    _ = MessageBox.Show(error.Message);
                }
            }

            var thirdPartyFiles = SelectedThirdPartyFiles();
            foreach (var file in thirdPartyFiles)
            {
                try
                {
                    var filename = Path.GetRelativePath(file.BasePath, file.Path);
                    Output.Write($"Installing: {filename}", Output.Level.Info);
                    VirtualPackage.Install(filename, file.BasePath, destination, makeCopy, warnMissingFile);
                }
                catch (SymLinkerError error)
                {
                    Output.Write($"Unable to copy file {error.Message}", Output.Level.Error);
                    _ = MessageBox.Show(error.Message);
                }
            }
            Output.Write("Install to virtual folder complete.", Output.Level.Status);
        }

        private IEnumerable<InstalledPackage> SelectedPackages ()
        {
            return packages.Packages.Where(x => x.Selected);
        }
        private IEnumerable<ThirdPartyEntry> SelectedThirdPartyFiles()
        {
            return ThirdParty.AllSelected();
        }

        public class SelectionRecords
        {
            public List<string> PackageNames { get; set; }
            public List<string> ThirdPartyFilenames { get; set; }
        }
        public  void SaveSelectionsToFile ()
        {
            var (success, file) = SelectFile.AskForSaveLocation();
            if (success)
            {
                try
                {
                    var option = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    var selectedPackages = SelectedPackages().Select(x => x.ProductName).ToList();
                    var selectedFiles = SelectedThirdPartyFiles().Select(x => x.Path).ToList();
                    var records = new SelectionRecords() { PackageNames = selectedPackages, ThirdPartyFilenames = selectedFiles };
                    File.WriteAllText(file, JsonSerializer.Serialize(records, option));
                }
                catch (IOException error)
                {
                    Output.Write($"Unable to write to {file}. Error: {error.Message}", Output.Level.Error);
                }
            }
        }

        public void LoadSelectionsFromFile ()
        {
            var (success, file) = SelectFile.AskForOpenLocation();
            if (success)
            {
                try
                {
                    Output.Write($"Reading selections from {file}", Output.Level.Status);
                    var option = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    using var jsonFile = File.OpenText(file);
                    var records = JsonSerializer.Deserialize<SelectionRecords>(jsonFile.ReadToEnd(), option);
                    packages.SelectPackages(records.PackageNames);
                    ThirdParty.SelectFiles(records.ThirdPartyFilenames);
                    foreach (var name in records.PackageNames)
                    {
                        Output.Write($"{name}", Output.Level.Info);
                    }
                    foreach (var name in records.ThirdPartyFilenames)
                    {
                        Output.Write($"{name}", Output.Level.Info);
                    }

                }
                catch (JsonException)
                {
                    Output.Write($"Unable load from {file}.", Output.Level.Warning);
                }
                catch (FileNotFoundException)
                {
                }
                catch (IOException error)
                {
                    Output.Write($"Unable to read {file}. Error: {error.Message}", Output.Level.Error);
                }
            }
        }

        #region DAZ Folders
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
                Output.Write($"Error source: {error.InnerException.Source}", Output.Level.Error);
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
        #endregion

        #region Third Party Contents
        public CollectionViewSource ThirdPartyFoldersSource { get; private set; } = new();
        public ThirdPartyFolders ThirdParty { get; private set; } = new();

        private CancellationTokenSource OtherPartyToken = null;
        public void CancelThirdPartyProcess()
        {
            Output.Write("Canceling processing 3rd party folders.", Output.Level.Status);
            OtherPartyToken.Cancel();
        }

        public async Task AddThirdPartyFolder()
        {
            var (success, folder) = SelectFolder.AskForLocation();
            if (success)
            {
                try
                {
                    Output.Write($"Scanning 3rd party folder: {folder}.\n Please Wait.", Output.Level.Status);
                    OtherPartyToken = new();
                    await Task.Run(() => ThirdParty.AddFolder(folder, OtherPartyToken.Token), OtherPartyToken.Token);
                    SaveThirdPartyFolders();
                    Output.Write($"Finished scanning 3rd party folder: {folder}.", Output.Level.Status);
                }
                catch (TargetInvocationException error)
                {
                    Output.Write($"Error source: {error.InnerException.Source}", Output.Level.Error);
                    Output.Write($"Error error message: {error.InnerException.Message}", Output.Level.Error);
                }
                catch (OperationCanceledException)
                {
                    Output.Write($"Scanning 3rd party folder task canceled.", Output.Level.Status);
                }
                finally
                {
                    OtherPartyToken.Dispose();
                }
            }
        }

        public async Task ReloadThirdPartyFolder()
        {
            try
            {
                Output.Write($"Reloading 3rd party folders.", Output.Level.Status);
                OtherPartyToken = new();
                await Task.Run(() => ThirdParty.ReloadFolders(OtherPartyToken.Token), OtherPartyToken.Token);
                SaveThirdPartyFolders();
                Output.Write($"Finished reloading 3rd party folders.", Output.Level.Status);
            }
            catch (TargetInvocationException error)
            {
                Output.Write($"Error source: {error.InnerException.Source}", Output.Level.Error);
                Output.Write($"Error error message: {error.InnerException.Message}", Output.Level.Error);
            }
            catch (OperationCanceledException)
            {
                Output.Write($"Scanning 3rd party folder task canceled.", Output.Level.Status);
            }
            finally
            {
                OtherPartyToken.Dispose();
            }
        }

        public void RemoveThirdPartyFolder(int index)
        {
            ThirdParty.RemoveFolder(index);
            SaveThirdPartyFolders();
        }

        private void SaveThirdPartyFolders()
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var savePath = SaveFileLocation(thirdPartyFolderJsonFile);
            File.WriteAllText(savePath, JsonSerializer.Serialize(ThirdParty.Folders, option));

            savePath = SaveFileLocation(thirdPartyFilesJsonFile);
            File.WriteAllText(savePath, JsonSerializer.Serialize(ThirdParty.Files, option));
        }

        public void LoadThirdPartyFolders()
        {
            var saveFileLocation = SaveFileLocation(thirdPartyFolderJsonFile);
            try
            {
                var option = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                using var jsonFile = File.OpenText(saveFileLocation);
                try
                {
                    var folders = JsonSerializer.Deserialize<List<string>>(jsonFile.ReadToEnd(), option);
                    jsonFile.Dispose();
                    ThirdParty.Folders.AddRange(folders);

                    saveFileLocation = SaveFileLocation(thirdPartyFilesJsonFile);
                    using var jsonFile2 = File.OpenText(saveFileLocation);
                    var files = JsonSerializer.Deserialize<List<ThirdPartyFolder>>(jsonFile2.ReadToEnd(), option);
                    ThirdParty.Files = files;
                    jsonFile2.Dispose();
                    UpdateThirdPartyView(); //TODO tidy up.
                }
                catch (JsonException)
                {
                    Output.Write("Unable to load cache file. Clearing Cache.", Output.Level.Warning);
                    jsonFile.Dispose();
                    File.Delete(saveFileLocation);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        public CollectionViewSource ThirdPartyView { get; set; } = new();
        private void UpdateThirdPartyView(object sender, PropertyChangedEventArgs e)
        {
            UpdateThirdPartyView();
        }
        private void UpdateThirdPartyView()
        {
            ThirdPartyView.Source = ThirdParty.AllFiles().ToList();
        }
        #endregion

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
            var savePath = SaveFileLocation(archiveJsonFile);
            File.WriteAllText(savePath, JsonSerializer.Serialize(packages.Packages, option));
        }

        public void LoadPackagesCache()
        {
            var saveFileLocation = SaveFileLocation(archiveJsonFile);
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

        private const string archiveJsonFile = "Archive.json";
        private const string thirdPartyFolderJsonFile = "3rdPartyFolders.json";
        private const string thirdPartyFilesJsonFile = "3rdPartyFiles.json";

        private static string SaveFileLocation(string filename)
        {
            return Path.Combine(Properties.Settings.Default.CacheLocation, filename);
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

        public CollectionViewSource PackagesViewSource { get; set; } = new();
        public CollectionViewSource Accessories { get; set; } = new();
        public CollectionViewSource Attachments { get; set; } = new();
        public CollectionViewSource Characters { get; set; } = new();
        public CollectionViewSource Clothings { get; set; } = new();
        public CollectionViewSource Hairs { get; set; } = new();
        public CollectionViewSource Morphs { get; set; } = new();
        public CollectionViewSource Props { get; set; } = new();
        public CollectionViewSource Poses { get; set; } = new();
        public CollectionViewSource Others { get; set; } = new();
        public CollectionViewSource TODO { get; set; } = new();

        private static readonly GenerationToStringConverter generationToStringConverter = new();
        private static readonly GenerationGroupCompare generationGroupCompare = new();
        private static readonly PropertyGroupDescription generationGrouping = new("Generations", generationToStringConverter)
        {
            CustomSort = generationGroupCompare
        };

        private static readonly StringCompareHelper stringCompare = new();
        private static readonly AssetToStringConverter installedPackageAssetTypeConverter = new();
        private static readonly PropertyGroupDescription itemAssetTypeGrouping = new("AssetTypes", installedPackageAssetTypeConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly GenderToStringConverter genderToStringConverter = new();
        private static readonly PropertyGroupDescription genderGrouping = new("Genders", genderToStringConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly ContentTypeToDisplayConverter installedItemContentTypeConverter = new();
        private static readonly PropertyGroupDescription itemContentGrouping = new("ContentType", installedItemContentTypeConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly CategoriesConverter installedItemCategoriesConverter = new();
        private static readonly PropertyGroupDescription itemCategoriesGrouping = new("Categories", installedItemCategoriesConverter)
        {
            CustomSort = stringCompare
        };
        #endregion

        #region Gui Settings
        private GUISettings settings = new();
        public GUISettings Settings
        {
            get => settings;
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
