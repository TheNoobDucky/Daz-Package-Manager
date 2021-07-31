using DazPackage;
using OsHelper;
using Output;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Daz_Package_Manager
{
    internal class VirtualFolderInstaller
    {
        private readonly Backend model;

        public VirtualFolderInstaller(Backend model)
        {
            this.model = model;
        }
        public async Task Install(string destination, bool makeCopy = false, bool warnMissingFile = false)
        {
            try
            {
                tokenSource = new();
                await Task.Run(() => Install_Imple(destination, tokenSource.Token, makeCopy, warnMissingFile), tokenSource.Token);
            }
            catch (TargetInvocationException error)
            {
                InfoBox.Write($"Invoke error Error source: {error.InnerException.Source}", InfoBox.Level.Error);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        private Task Install_Imple (string destination, CancellationToken token, bool makeCopy = false, bool warnMissingFile = false)
        {
            if (destination is null or "")
            {
                InfoBox.Write("Please select a location to install virtual packages to.", InfoBox.Level.Error);
                return Task.CompletedTask;
            }

            _ = Directory.CreateDirectory(destination);

            InfoBox.Write("Installing to virtual folder location: " + destination, InfoBox.Level.Status);

            var packagesToSave = model.Packages.AllSelected();
            foreach (var package in packagesToSave)
            {
                token.ThrowIfCancellationRequested();
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
                token.ThrowIfCancellationRequested();
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
            return Task.CompletedTask;
        }

        private CancellationTokenSource tokenSource = null;
        public void Cancel()
        {
            InfoBox.Write("Canceling installing virtual folder task.", InfoBox.Level.Status);
            tokenSource.Cancel();
        }
    }
}
