using DazPackage;
using OsHelper;
using Output;
using System.Windows;

namespace Daz_Package_Manager
{
    internal class VirtualFolderManager
    {
        private readonly Backend model;

        public VirtualFolderManager(Backend model)
        {
            this.model = model;
        }
        public void Install(string destination, bool makeCopy = false, bool warnMissingFile = false)
        {
            if (destination is null or "")
            {
                InfoBox.Write("Please select a location to install virtual packages to.", InfoBox.Level.Error);
                return;
            }

            InfoBox.Write("Installing to virtual folder location: " + destination, InfoBox.Level.Status);

            var packagesToSave = model.Packages.AllSelected();
            foreach (var package in packagesToSave)
            {
                InfoBox.Write("Installing: " + package.ProductName, InfoBox.Level.Info);
                try
                {
                    VirtualPackage.Install(package, destination, makeCopy, warnMissingFile);
                }
                catch (SymLinkerError error)
                {
                    InfoBox.Write($"Unable to copy file {error.Message}", InfoBox.Level.Error);
                    _ = MessageBox.Show(error.Message);
                }
            }

            var thirdPartyFiles = model.ThirdParty.AllSelected();
            foreach (var file in thirdPartyFiles)
            {
                try
                {
                    InfoBox.Write($"Installing: {file.RelativePath}", InfoBox.Level.Info);
                    VirtualPackage.Install(file.RelativePath, file.ParentFolder.BasePath, destination, makeCopy, warnMissingFile);
                }
                catch (SymLinkerError error)
                {
                    InfoBox.Write($"Unable to copy file {error.Message}", InfoBox.Level.Error);
                    _ = MessageBox.Show(error.Message);
                }
            }
            InfoBox.Write("Install to virtual folder complete.", InfoBox.Level.Status);
        }
    }
}
