using DazPackage;
using OsHelper;
using Output;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Daz_Package_Manager
{
    internal class PackagesSaveDetail
    {
        public string Name { get; set; }
        public string ProductID { get; set; }
        public string PackageID { get; set; }

        public PackagesSaveDetail()
        {

        }
        public PackagesSaveDetail(InstalledPackage package) 
        {
            Name = package.ProductName;
            ProductID = package.ProductID;
            PackageID = package.PackageID;
        }
    }

    internal class SelectionRecords
    {
        public List<PackagesSaveDetail> Packages { get; set; }
        public List<string> ThirdPartyFilenames { get; set; }
    }

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

        public async Task LoadAllCaches ()
        {
            Task[] tasks = { model.ManifestScanner.LoadCache(), model.ThirdPartyScanner.LoadCache() };
            await Task.WhenAll(tasks);
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
                    model.Packages.SelectPackages(records.Packages.Select(x => x.Name).ToList());
                    model.ThirdParty.SelectFiles(records.ThirdPartyFilenames);
                    foreach (var name in records.Packages)
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
                    var selectedPackages = model.Packages.AllSelected().Select(x => new PackagesSaveDetail(x)).ToList();
                    var selectedFiles = model.ThirdParty.AllSelected().Select(x => x.Location).ToList();
                    var records = new SelectionRecords() { Packages = selectedPackages, ThirdPartyFilenames = selectedFiles };
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
