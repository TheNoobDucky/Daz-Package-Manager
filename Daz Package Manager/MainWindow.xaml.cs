using DazPackage;
using Output;
using OsHelper;
using System;
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
            InfoBox.RegisterDebugField(DebugText);
            InfoBox.WriteDebug = true;
            DataContext = modelView;
            modelView.LoadPackagesCache();
        }

        private readonly ModelView modelView = new();
        private const string waitText = "Cancel";

        private void GenerateVirtualInstallFolder(object sender, RoutedEventArgs e)
        {
            var destination = InstallFolder();
            Directory.CreateDirectory(destination);
            var makeCopy = Properties.Settings.Default.MakeCopy;
            var warnMissingFile = Properties.Settings.Default.WarnMissingFile;
            modelView.GenerateVirtualInstallFolder(destination, makeCopy, warnMissingFile);
        }

        private static string InstallFolder()
        {
            var destination = Properties.Settings.Default.OutputFolder;
            if (Properties.Settings.Default.UseSceneSubfolder)
            {
                destination = Path.Combine(destination, SceneName());
            }
            return destination;
        }

        private static string SceneName()
        {
            return Path.GetFileNameWithoutExtension(Properties.Settings.Default.SceneFile);
        }

        private void GenerateInstallScript(object sender, RoutedEventArgs e)
        {
            var virtualFolder = InstallFolder();

            var scene = Properties.Settings.Default.SceneFile;
            var sceneRoot = Directory.GetParent(scene);
            var scriptName = Path.GetFileNameWithoutExtension(scene) + "_load.dsa";
            var scriptLocation = Path.Combine(sceneRoot.FullName, scriptName);

            VirtualPackage.SaveInstallScript(scriptLocation, virtualFolder, scene);
        }

        private async void ScanInstallManifestFolder(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                if (button.Content is string prev_text)
                {
                    if (prev_text == waitText)
                    {
                        modelView.CancelManifestScan();
                        return;
                    }

                    button.Content = waitText;
                    await modelView.ScanManifestFolder();
                    button.Content = prev_text;
                }
            }
        }

        private void SelectFigureBasedOnScene(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.BatchProcessScene)
            {
                modelView.SelectPackagesBasedOnFolder(Properties.Settings.Default.SceneFile);
            }
            else
            {
                modelView.SelectPackagesBasedOnScene(Properties.Settings.Default.SceneFile);
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
            var (success, location) = SelectFile.AskForOpenLocation();
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
            modelView.UnselectAll();
        }

        private void CallLoadCache(object sender, RoutedEventArgs e)
        {
            modelView.LoadPackagesCache();
        }

        private void SaveUserSetting(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private async void Add3rdPartyFolder(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                if (button.Content is string prevText)
                {
                    if (prevText == waitText)
                    {
                        modelView.CancelThirdPartyProcess();
                        return;
                    }
                    var reloadContent = ReloadThirdPartyButton.Content;
                    var removeContent = RemoveThirdPartyButton.Content;

                    button.Content = waitText;
                    ReloadThirdPartyButton.Content = waitText;
                    RemoveThirdPartyButton.Content = waitText;
                    await modelView.AddThirdPartyFolder();
                    button.Content = prevText;
                    ReloadThirdPartyButton.Content = reloadContent;
                    RemoveThirdPartyButton.Content = removeContent;
                }
            }
        }

        private void Remove3rdPartyFolder(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                if (button.Content is string prevText)
                {
                    if (prevText == waitText)
                    {
                        modelView.CancelThirdPartyProcess();
                        return;
                    }
                    var index = OtherPartyFolders.SelectedIndex;
                    modelView.RemoveThirdPartyFolder(index);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            modelView.LoadThirdPartyFolders();
        }

        private async void Reload3rdPartyFolder(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                if (button.Content is string prevText)
                {
                    if (prevText == waitText)
                    {
                        modelView.CancelThirdPartyProcess();
                        return;
                    }
                    var addContent = AddThirdPartyButton.Content;
                    var removeContent = RemoveThirdPartyButton.Content;

                    button.Content = waitText;
                    AddThirdPartyButton.Content = waitText;
                    RemoveThirdPartyButton.Content = waitText;
                    await modelView.ReloadThirdPartyFolder();
                    button.Content = prevText;
                    AddThirdPartyButton.Content = addContent;
                    RemoveThirdPartyButton.Content = removeContent;
                }
            }
        }

        private void SaveSelection(object sender, RoutedEventArgs e)
        {
            modelView.SaveSelectionsToFile();
        }

        private void LoadSelection(object sender, RoutedEventArgs e)
        {
            modelView.LoadSelectionsFromFile();
        }
    }
}
