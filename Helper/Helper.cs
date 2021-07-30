using Output;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Helpers
{
    public class Helper
    {
        public static void DeleteEmptyFolder(string folder)
        {
            if (!Directory.EnumerateFileSystemEntries(folder).Any())
            {
                try
                {
                    InfoBox.Write("Deleting empty folder: " + folder, InfoBox.Level.Info, 60.0);
                    Directory.Delete(folder);
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
                catch (IOException)
                { //block not empty?
                }
            }
        }

        public static void MoveFolder(string source, string destination)
        {
            try
            {
                var parent = new FileInfo(destination).Directory;
                if (!parent.Exists)
                {
                    parent.Create();
                }

                Directory.Move(source, destination);
            }
            catch (Exception e)
            {
                InfoBox.Write(e.Message, InfoBox.Level.Error);
            }
        }

        public static void SafeMove(string source, string destination)
        {
            SafeMove(new FileInfo(source), destination);
        }

        public static void SafeMove(FileInfo source, string destination)
        {
            if (!File.Exists(destination))
            {
                source.MoveTo(destination, false);
            }
            else
            {
                var sourceHash = CalculateMD5Hash(source);
                var destinationHash = CalculateMD5Hash(new FileInfo(destination));
                if (sourceHash.SequenceEqual(destinationHash))
                {
                    InfoBox.Write("Identical file, deleting: " + source.Name, InfoBox.Level.Alert, 20.0);
                    try
                    {
                        source.Delete();
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
                else
                {
                    InfoBox.Write("Non Identical file, skipping: " + source.Name, InfoBox.Level.Warning, 0.0);
                }
            }
        }

        public static Byte[] CalculateMD5Hash(FileInfo file)
        {
            using var sourceStream = file.OpenRead();
            using var md5 = System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(sourceStream);
        }

        public static JsonDocument ReadJsonFromGZfile(FileInfo file)
        {
            using var sceneStream = file.OpenRead();
            using var scene = new GZipStream(sceneStream, CompressionMode.Decompress);
            return JsonDocument.Parse(scene);
        }

        public static JsonDocument ReadJsonFromTextFile(FileInfo file)
        {
            using var sceneStream = file.OpenRead();
            //using var scene = new FileStream(sceneStream, CompressionMode.Decompress);
            return JsonDocument.Parse(sceneStream);
        }

        public static void TriggerFilterRefresh(ItemsControl dataGrid)
        {
            if (dataGrid != null && dataGrid.ItemsSource != null)
            {
                //CollectionViewSource.GetDefaultView(dataGrid.ItemsSource).Refresh();
            }
        }

        private void TabChangeHandler(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl)
            {
                var tabItem = e.AddedItems[0] as TabItem;
                Helper.TriggerFilterRefresh(tabItem.Content as ItemsControl);
            }
        }

        private void UpdateDisplayHandler(object sender, RoutedEventArgs e)
        {
            //var tabItem = DisplayTab.SelectedItem as TabItem;
            //Helper.TriggerFilterRefresh(tabItem.Content as ItemsControl);
        }
    }
}
