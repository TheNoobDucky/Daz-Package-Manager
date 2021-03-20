using System;
using System.Collections.Generic;
using System.Windows;
using Helpers;
using DazPackage;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Windows.Data;

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
            public WindowModel()
            {
            }

            private List<InstalledPackage> packages = new List<InstalledPackage>();
            public List<InstalledPackage> Packages { get => packages; set { packages = value; PackagesViewSource.Source = packages; } }
            public CollectionViewSource PackagesViewSource { get; set; } = new CollectionViewSource();

            public List<InstalledCharacter> characters = new List<InstalledCharacter>();
            public List<InstalledCharacter> Characters { get => characters; set { characters = value; CharactersViewSource.Source = characters; } }
            public CollectionViewSource CharactersViewSource { get; set; } = new CollectionViewSource();

            public List<InstalledPose> poses = new List<InstalledPose>();
            public List<InstalledPose> Poses { get => poses; set { poses = value; PosesViewSource.Source = poses; } }
            public CollectionViewSource PosesViewSource { get; set; } = new CollectionViewSource();
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
            SaveCache();
        }

        private void LoadCache()
        {
            try
            {
                using var packageJsonFile = File.OpenText(packagesFile);
                model.Packages = JsonSerializer.Deserialize<List<InstalledPackage>>(packageJsonFile.ReadToEnd());
                (model.Characters, model.Poses) = ProcessInstallManifestFolder.GenerateItemLists(model.Packages);
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

        private string packagesFile
        {
            get
            {
                return Path.Combine(Properties.Settings.Default.CacheLocation, "Packages.json");
            }
        }

        private void RefreshAllDisplay()
        {
            PackageDisplay.Items.Refresh();
            CharactersDisplay.Items.Refresh();
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
            RefreshAllDisplay();
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
