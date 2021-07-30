using DazPackage;
using Output;
using OsHelper;
using System;
using System.IO;
using System.Windows;
using Helpers;

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
            if (Properties.Settings.Default.CacheLocation == string.Empty)
            {
                var userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Properties.Settings.Default.CacheLocation = Path.Combine(userPath, "Daz Package Manager");
                _ = Directory.CreateDirectory(Properties.Settings.Default.CacheLocation);
            }
            InfoBox.RegisterDebugField(DebugText);
            InfoBox.WriteDebug = true;
            DataContext = modelView;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var scanContent = ScanInstallManifestFolderButton.Content;
            ScanInstallManifestFolderButton.Content = Helper.WaitText;
            var ThirdPartyContent = AddThirdPartyButton.Content;
            AddThirdPartyButton.Content = Helper.WaitText;

            await modelView.CacheManager.LoadAllCaches();

            ScanInstallManifestFolderButton.Content = scanContent;
            AddThirdPartyButton.Content = ThirdPartyContent;
        }

        private readonly Backend modelView = new();

        private async void GenerateVirtualInstallFolder(object sender, RoutedEventArgs e)
        {
            await Helper.AsyncButton(sender,async ()=> 
            
            {
                var destination = InstallFolder();
                var makeCopy = Properties.Settings.Default.MakeCopy;
                var warnMissingFile = Properties.Settings.Default.WarnMissingFile;
                await modelView.VirtualFolderManager.Install(destination, makeCopy, warnMissingFile);
            }, ()=> { });
        }

        private static string InstallFolder()
        {
            var destination = Properties.Settings.Default.OutputFolder;
            if (destination is null or "")
            {
                return null;
            }

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
            await Helper.AsyncButton(sender, 
                async () => await modelView.ManifestScanner.Scan(), 
                () => modelView.ManifestScanner.Cancel());
        }

        private async void SelectPackagesBasedOnScene(object sender, RoutedEventArgs e)
        {
            await Helper.AsyncButton(sender,
                async () =>
                {
                    if (Properties.Settings.Default.BatchProcessScene)
                    {
                        await modelView.SelectPackages.BasedOnFolder(Properties.Settings.Default.SceneFile);
                    }
                    else
                    {
                        await modelView.SelectPackages.BasedOnScene(Properties.Settings.Default.SceneFile);
                    }
                },
                () => modelView.SelectPackages.Cancel()
            );
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
            modelView.Packages.UnselectAll();
        }

        private async void CallLoadCache(object sender, RoutedEventArgs e)
        {
            await Helper.AsyncButton(sender,
                async () => await modelView.ManifestScanner.LoadCache(),
                () => modelView.ManifestScanner.Cancel());
        }

        private void SaveUserSetting(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private async void Add3rdPartyFolder(object sender, RoutedEventArgs e)
        {
            await Helper.AsyncButton(sender, async () =>
            {
                var reloadContent = ReloadThirdPartyButton.Content;
                var removeContent = RemoveThirdPartyButton.Content;
                ReloadThirdPartyButton.Content = Helper.WaitText;
                RemoveThirdPartyButton.Content = Helper.WaitText;
                await modelView.ThirdPartyScanner.AddFolder();
                ReloadThirdPartyButton.Content = reloadContent;
                RemoveThirdPartyButton.Content = removeContent;
            },
                () => modelView.ThirdPartyScanner.Cancel()
            );
        }

        private void Remove3rdPartyFolder(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                if (button.Content is string prevText)
                {
                    if (prevText == Helper.WaitText)
                    {
                        modelView.ThirdPartyScanner.Cancel();
                        return;
                    }
                    var index = OtherPartyFolders.SelectedIndex;
                    modelView.ThirdPartyScanner.RemoveFolder(index);
                }
            }
        }

        private async void Reload3rdPartyFolder(object sender, RoutedEventArgs e)
        {
            await Helper.AsyncButton(sender, async () =>
            {
                var addContent = AddThirdPartyButton.Content;
                var removeContent = RemoveThirdPartyButton.Content;

                AddThirdPartyButton.Content = Helper.WaitText;
                RemoveThirdPartyButton.Content = Helper.WaitText;
                await modelView.ThirdPartyScanner.ReloadThirdPartyFolder();
                AddThirdPartyButton.Content = addContent;
                RemoveThirdPartyButton.Content = removeContent;
            },
                () => modelView.ThirdPartyScanner.Cancel()
            );
        }

        private void SaveSelection(object sender, RoutedEventArgs e)
        {
            modelView.CacheManager.SaveSelectionsToFile();
        }

        private void LoadSelection(object sender, RoutedEventArgs e)
        {
            modelView.CacheManager.LoadSelectionsFromFile();
        }
    }
}
