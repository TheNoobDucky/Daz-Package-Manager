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
        public async Task Install(string destination, bool makeCopy = false, bool ignoreMissingFile = false)
        {
            try
            {
                tokenSource = new();
                await Task.Run(() => Install_Imple(destination, tokenSource.Token, makeCopy, ignoreMissingFile), tokenSource.Token);
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

        private Task Install_Imple(string destination, CancellationToken token, bool makeCopy = false, bool ignoreMissingFile = false)
        {
            try
            {
                _ = Directory.CreateDirectory(destination);

                InfoBox.Write("Installing to virtual folder location: " + destination, InfoBox.Level.Status);

                var packagesToSave = model.Packages.AllSelected();
                foreach (var package in packagesToSave)
                {
                    token.ThrowIfCancellationRequested();
                    InfoBox.Write("Installing: " + package.ProductName, InfoBox.Level.Info);
                    try
                    {
                        VirtualPackage.Install(package, destination, makeCopy, ignoreMissingFile);
                    }
                    catch (SymLinkerError error)
                    {
                        InfoBox.Write($"Unable to copy file {error.Message}", InfoBox.Level.Error);
                        var buttons = MessageBoxButton.YesNo;
                        var result = MessageBox.Show(error.Message + "\n\nAbort?", "Cancel Creating Virtual Folder?", buttons);
                        if (result == MessageBoxResult.Yes)
                        {
                            InfoBox.Write($"Cancelling virtual folder operation.", InfoBox.Level.Error);
                            return Task.CompletedTask;
                        }
                    }
                }

                var thirdPartyFiles = model.ThirdParty.AllSelected();
                foreach (var file in thirdPartyFiles)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        InfoBox.Write($"Installing: {file.RelativePath}", InfoBox.Level.Info);
                        VirtualPackage.Install(file.RelativePath, file.ParentFolder.BasePath, destination, makeCopy, ignoreMissingFile);
                    }
                    catch (SymLinkerError error)
                    {
                        InfoBox.Write($"Unable to copy file {error.Message}", InfoBox.Level.Error);
                        var buttons = MessageBoxButton.YesNo;
                        var result = MessageBox.Show(error.Message + "\n\nAbort?", "Cancel installing to virtual folder?", buttons);
                        if (result == MessageBoxResult.Yes)
                        {
                            InfoBox.Write($"Cancelling virtual folder operation.", InfoBox.Level.Error);
                            return Task.CompletedTask;
                        }
                    }
                }
                InfoBox.Write("Install to virtual folder complete.", InfoBox.Level.Status);
            }
            catch (ArgumentException)
            {
                InfoBox.Write("Please select virtual package install location.", InfoBox.Level.Error);
            }
            return Task.CompletedTask;
        }

        private CancellationTokenSource tokenSource;
        public void Cancel()
        {
            InfoBox.Write("Canceling installing virtual folder task.", InfoBox.Level.Status);
            tokenSource.Cancel();
        }
    }
}
