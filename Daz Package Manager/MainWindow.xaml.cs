using Helpers;
using OsHelper;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

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
            model.PropertyChanged += ScanCompleted;
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
            }
            model.GenerateVirtualInstallFolder(destination);
        }

        private void ScanInstallManifestFolder(object sender, RoutedEventArgs e)
        {
            if (!model.Working)
            {
                model.Scan();
            }
        }

        private void ScanCompleted(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Working")
            {
                var model = (ProcessModel)sender;
                if (model.Working)
                {
                    ScanInstallManifestFolderButton.Content = "Waiting For Scan To Complete.";
                }
                else
                {
                    ScanInstallManifestFolderButton.Content = "Scan Install Manifest Archive."; // TODO improve this.
                }
            }
        }

        private void SelectFigureBasedOnScene(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.BatchProcessScene)
            {
                model.SelectPackageBasedOnFolder(Properties.Settings.Default.SceneFile);
            }
            else
            {
                model.SelectPackageBasedOnScene(Properties.Settings.Default.SceneFile);
            }
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
        private static TraceSource ts = new TraceSource("TraceTest");

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
