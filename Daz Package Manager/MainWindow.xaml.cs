using System;
using System.Windows;
using Helpers;
using System.IO;
using OsHelper;

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
            model.LoadCache(Properties.Settings.Default.CacheLocation);
        }

        readonly ProcessModel model = new ProcessModel();


        private void GenerateVirtualInstallFolder(object sender, RoutedEventArgs e)
        {
            var destination = Properties.Settings.Default.OutputFolder;
            if (Properties.Settings.Default.UseSceneSubfolder)
            {
                destination = Path.Combine(destination, Path.GetFileNameWithoutExtension(Properties.Settings.Default.SceneFile));
                Directory.CreateDirectory(destination);
                Output.Write(destination);
            }
            model.GenerateVirtualInstallFolder(destination);
        }

        private void ScanInstallManifestFolder(object sender, RoutedEventArgs e)
        {
            model.Archive = ProcessModel.Scan();
            model.SaveCache(Properties.Settings.Default.CacheLocation);
        }

        private void SelectFigureBasedOnScene(object sender, RoutedEventArgs e)
        {
            model.SelectFigureBasedOnScene();
        }

        // Below are boring functions.
        private void SelectOutputFolder(object sender, RoutedEventArgs e)
        {
            var (success, location) = SelectFolder.AskForLocation();

            if (success)
            {
                Properties.Settings.Default.OutputFolder = location;
                Properties.Settings.Default.Save();
            }
        }

        private void SelectInstallManifestFolder(object sender, RoutedEventArgs e)
        {
            var (success, location) = SelectFolder.AskForLocation();

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
            var (success, location) = SelectFile.AskForLocation();
            if (success)
            {
                Properties.Settings.Default.SceneFile = location;
                Properties.Settings.Default.Save();
            }
        }

        private void SelectCacheLocation(object sender, RoutedEventArgs e)
        {
            var (success, location) = SelectFolder.AskForLocation();

            if (success)
            {
                Properties.Settings.Default.CacheLocation = location;
                Properties.Settings.Default.Save();
                Directory.CreateDirectory(Properties.Settings.Default.CacheLocation);
            }
        }

        private void ClearPackageSelection(object sender, RoutedEventArgs e)
        {
            model.UnselectAll();
        }

        private void CallLoadCache(object sender, RoutedEventArgs e)
        {
            model.LoadCache(Properties.Settings.Default.CacheLocation);
        }

        private void SaveUserSetting(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
