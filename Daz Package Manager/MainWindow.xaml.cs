using System;
using System.Collections.Generic;
using System.Windows;
using Helpers;
using DazPackage;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace Daz_Package_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Properties.Settings.Default.CacheLocation == String.Empty)
            {
                var userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Properties.Settings.Default.CacheLocation = Path.Combine(userPath, "Daz Package Manager");
                Directory.CreateDirectory(Properties.Settings.Default.CacheLocation);
            }
            Output.RegisterDebugField(DebugText);
            Output.WriteDebug = true;
            DataContext = model;
            LoadCache();
        }

        readonly WindowModel model = new WindowModel();

        public class WindowModel
        {
            public List<InstalledPackage> Packages { get; set; } = new List<InstalledPackage>();
            public List<InstalledCharacter> Characters { get; set; } = new List<InstalledCharacter>();
        }

        private void GenerateVirtualInstallFolder(object sender, RoutedEventArgs e)
        {
            var destination = Properties.Settings.Default.OutputFolder;
            if (Properties.Settings.Default.UseSceneSubfolder)
            {
                destination = Path.Combine(destination, Path.GetFileNameWithoutExtension(Properties.Settings.Default.SceneFile));
                Directory.CreateDirectory(destination);
                Output.Write(destination);
            }
            VirtualInstall.InstallPackages(model.Packages.Where(x => x.Selected), destination);
        }

        private void ScanInstallManifestFolder(object sender, RoutedEventArgs e)
        {
            (model.Packages, model.Characters) = ProcessInstallManifestFolder.Scan();
            UpdateDisplay();
            SaveCache();
        }

        private void LoadCache()
        {
            try
            {
                using var packageJsonFile = File.OpenText(packagesFile);
                model.Packages = JsonSerializer.Deserialize<List<InstalledPackage>>(packageJsonFile.ReadToEnd());
                model.Characters = ProcessInstallManifestFolder.GenerateItemLists(model.Packages);
                UpdateDisplay();
            }
            catch (FileNotFoundException)
            {
            }
        }

        private void SaveCache()
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            File.WriteAllText(packagesFile, JsonSerializer.Serialize(model.Packages, option));
        }

        private void UpdateDisplay()
        {
            PackageDisplay.ItemsSource = model.Packages;
            CharactersDisplay.ItemsSource = model.Characters;
        }

        private string packagesFile
        {
            get
            {
                return Path.Combine(Properties.Settings.Default.CacheLocation, "Packages.json");
            }
        }

        // Below are boring functions.
        private void SelectFigureBasedOnScene(object sender, RoutedEventArgs e)
        {
            var sceneLocation = new FileInfo(Properties.Settings.Default.SceneFile);
            var packagesInScene = SceneFile.PackageInScene(sceneLocation, model.Packages);
            packagesInScene.ToList().ForEach(x => x.Selected = true);
        }

        private void SelectOutputFolder(object sender, RoutedEventArgs e)
        {
            var (success, location) = Helper.AskForFolderLocation();

            if (success)
            {
                Properties.Settings.Default.OutputFolder = location;
                Properties.Settings.Default.Save();
            }
        }

        private void SelectInstallManifestFolder(object sender, RoutedEventArgs e)
        {
            var (success, location) = Helper.AskForFolderLocation();

            if (success)
            {
                Properties.Settings.Default.InstallManifestFolder = location;
                Properties.Settings.Default.Save();
            }
        }

        private void ClearDebugLogHandler(object sender, RoutedEventArgs e)
        {
            DebugText.Blocks.Clear();
        }

        private void SelectSceneFile(object sender, RoutedEventArgs e)
        {
            var (success, location) = Helper.AskForFile();
            if (success)
            {
                Properties.Settings.Default.SceneFile = location;
                Properties.Settings.Default.Save();
            }
        }

        private void SelectCacheLocation(object sender, RoutedEventArgs e)
        {
            var (success, location) = Helper.AskForFolderLocation();

            if (success)
            {
                Properties.Settings.Default.CacheLocation = location;
                Properties.Settings.Default.Save();
                Directory.CreateDirectory(Properties.Settings.Default.CacheLocation);
            }
        }

        private void ClearPackageSelection(object sender, RoutedEventArgs e)
        {
            model.Packages.ForEach(x => x.Selected = false);
        }

        private void CallLoadCache(object sender, RoutedEventArgs e)
        {
            LoadCache();
        }

        private void SaveUserSetting(object sender, RoutedEventArgs e)
        {
            //Properties.Settings.Default.UseSceneSubfolder = !Properties.Settings.Default.UseSceneSubfolder;
            Properties.Settings.Default.Save();
        }
    }
}
