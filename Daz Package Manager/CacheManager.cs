using OsHelper;
using Output;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Daz_Package_Manager
{
    internal class CacheManager
    {
        private readonly Backend model;

        public CacheManager(Backend model)
        {
            this.model = model;
        }
        public static string SaveFileLocation(string filename)
        {
            return Path.Combine(Properties.Settings.Default.CacheLocation, filename);
        }

        public void LoadSelectionsFromFile()
        {
            var (success, file) = SelectFile.AskForOpenLocation();
            if (success)
            {
                try
                {
                    InfoBox.Write($"Reading selections from {file}", InfoBox.Level.Status);
                    var option = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    using var jsonFile = File.OpenText(file);
                    var records = JsonSerializer.Deserialize<SelectionRecords>(jsonFile.ReadToEnd(), option);
                    model.Packages.SelectPackages(records.PackageNames);
                    model.ThirdParty.SelectFiles(records.ThirdPartyFilenames);
                    foreach (var name in records.PackageNames)
                    {
                        InfoBox.Write($"{name}", InfoBox.Level.Info);
                    }
                    foreach (var name in records.ThirdPartyFilenames)
                    {
                        InfoBox.Write($"{name}", InfoBox.Level.Info);
                    }

                }
                catch (JsonException)
                {
                    InfoBox.Write($"Unable load from {file}.", InfoBox.Level.Warning);
                }
                catch (FileNotFoundException)
                {
                }
                catch (IOException error)
                {
                    InfoBox.Write($"Unable to read {file}. Error: {error.Message}", InfoBox.Level.Error);
                }
            }
        }
        public class SelectionRecords
        {
            public List<string> PackageNames { get; set; }
            public List<string> ThirdPartyFilenames { get; set; }
        }

        public void SaveSelectionsToFile()
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
                    var selectedPackages = model.Packages.AllSelected().Select(x => x.ProductName).ToList();
                    var selectedFiles = model.ThirdParty.AllSelected().Select(x => x.Location).ToList();
                    var records = new SelectionRecords() { PackageNames = selectedPackages, ThirdPartyFilenames = selectedFiles };
                    File.WriteAllText(file, JsonSerializer.Serialize(records, option));
                }
                catch (IOException error)
                {
                    InfoBox.Write($"Unable to write to {file}. Error: {error.Message}", InfoBox.Level.Error);
                }
            }
        }
    }
}
