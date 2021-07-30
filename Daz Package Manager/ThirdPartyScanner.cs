using DazPackage;
using OsHelper;
using Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Daz_Package_Manager
{
    internal class ThirdPartyScanner
    {
        private readonly Backend model;

        public ThirdPartyScanner(Backend model)
        {
            this.model = model;
        }

        private const string thirdPartyFolderJsonFile = "3rdPartyFolders.json";
        private const string thirdPartyFilesJsonFile = "3rdPartyFiles.json";

        public async Task AddFolder()
        {
            var (success, folder) = SelectFolder.AskForLocation();
            if (success)
            {
                try
                {
                    InfoBox.Write($"Scanning 3rd party folder: {folder}.\n Please Wait.", InfoBox.Level.Status);
                    OtherPartyToken = new();
                    await Task.Run(() => model.ThirdParty.AddFolder(folder, OtherPartyToken.Token), OtherPartyToken.Token);
                    SaveCache();
                    InfoBox.Write($"Finished scanning 3rd party folder: {folder}.", InfoBox.Level.Status);
                }
                catch (TargetInvocationException error)
                {
                    InfoBox.Write($"Error source: {error.InnerException.Source}", InfoBox.Level.Error);
                    InfoBox.Write($"Error error message: {error.InnerException.Message}", InfoBox.Level.Error);
                }
                catch (OperationCanceledException)
                {
                    InfoBox.Write($"Scanning 3rd party folder task canceled.", InfoBox.Level.Status);
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
                InfoBox.Write($"Reloading 3rd party folders.", InfoBox.Level.Status);
                OtherPartyToken = new();
                await Task.Run(() => model.ThirdParty.ReloadFolders(OtherPartyToken.Token), OtherPartyToken.Token);
                SaveCache();
                InfoBox.Write($"Finished reloading 3rd party folders.", InfoBox.Level.Status);
            }
            catch (TargetInvocationException error)
            {
                InfoBox.Write($"Error source: {error.InnerException.Source}", InfoBox.Level.Error);
                InfoBox.Write($"Error error message: {error.InnerException.Message}", InfoBox.Level.Error);
            }
            catch (OperationCanceledException)
            {
                InfoBox.Write($"Scanning 3rd party folder task canceled.", InfoBox.Level.Status);
            }
            finally
            {
                OtherPartyToken.Dispose();
            }
        }

        public void RemoveFolder(int index)
        {
            model.ThirdParty.RemoveFolder(index);
            SaveCache();
        }

        private void SaveCache()
        {
            var option = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var savePath = CacheManager.SaveFileLocation(thirdPartyFolderJsonFile);
            File.WriteAllText(savePath, JsonSerializer.Serialize(model.ThirdParty.Folders, option));

            savePath = CacheManager.SaveFileLocation(thirdPartyFilesJsonFile);
            File.WriteAllText(savePath, JsonSerializer.Serialize(model.ThirdParty.Files, option));
        }

        public void LoadCache()
        {
            var saveFileLocation = CacheManager.SaveFileLocation(thirdPartyFolderJsonFile);
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
                    model.ThirdParty.Folders.AddRange(folders);

                    saveFileLocation = CacheManager.SaveFileLocation(thirdPartyFilesJsonFile);
                    using var jsonFile2 = File.OpenText(saveFileLocation);
                    var files = JsonSerializer.Deserialize<List<ThirdPartyFolder>>(jsonFile2.ReadToEnd(), option);
                    model.ThirdParty.Files.Clear();
                    model.ThirdParty.Files.AddRange(files);
                    jsonFile2.Dispose();
                }
                catch (JsonException)
                {
                    InfoBox.Write("Unable to load cache file. Clearing Cache.", InfoBox.Level.Warning);
                    jsonFile.Dispose();
                    File.Delete(saveFileLocation);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        private CancellationTokenSource OtherPartyToken = null;
        public void Cancel()
        {
            InfoBox.Write("Canceling processing 3rd party folders.", InfoBox.Level.Status);
            OtherPartyToken.Cancel();
        }
    }
}
