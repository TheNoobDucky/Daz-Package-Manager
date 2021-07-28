using Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DazPackage
{
    public class ThirdPartyEntry : INotifyPropertyChanged
    {
        public string Path { get; set; }
        public string BasePath { get; set; }

        private bool selected = false;
        public bool IsDirectory { get; set; } = false;
        public bool Selected
        {
            get => selected;
            set
            {
                if (IsDirectory && Folder != null)
                {
                    Folder.Selected = value;
                }
                selected = value;
                OnPropertyChanged();
            }
        }
        public ThirdPartyFolder Folder { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public class ThirdPartyFolder
    {
        public bool Selected
        {
            set
            {
                foreach (var folder in Folders)
                {
                    folder.SelfEntry.Selected = value;
                }

                foreach (var file in Files)
                {
                    file.Selected = value;
                }
            }
        }

        public List<ThirdPartyEntry> Self { get; set; } = new();
        public ThirdPartyEntry SelfEntry { get => Self[0]; set => Self[0] = value; }
        public List<ThirdPartyEntry> Files { get; set; } = new();
        public List<ThirdPartyFolder> Folders { get; set; } = new();
        public string Folder { get; set; }
        public string BasePath { get; set; }

        public Task ScanFiles(CancellationToken token)
        {
            Self.Add(new ThirdPartyEntry { Folder = this, Path = Folder, BasePath = BasePath, IsDirectory = true });
            var subFolders = Directory.EnumerateDirectories(Folder);
            foreach (var subFolder in subFolders)
            {
                var newFolder = new ThirdPartyFolder { Folder = subFolder, BasePath = BasePath };
                Folders.Add(newFolder);
                newFolder.ScanFiles(token);
            }

            var files = Directory.EnumerateFiles(Folder);
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                Files.Add(item: new ThirdPartyEntry { Folder = this, Path = file, BasePath = BasePath, IsDirectory = false });

            }
            return Task.CompletedTask;
        }

        public IEnumerable<ThirdPartyEntry> GetAllFiles()
        {
            return Self.Concat(Folders.SelectMany(x => x.GetAllFiles())).Concat(Files);
        }
    }

    public class ThirdPartyFolders : INotifyPropertyChanged
    {
        public ObservableCollection<string> Folders { get; set; } = new();
        public List<ThirdPartyFolder> Files { get; set; } = new();

        public Task AddFolder(string folder, CancellationToken token)
        {
            Application.Current.Dispatcher.Invoke(() => Folders.Add(folder));
            var newFiles = new ThirdPartyFolder { Folder = folder, BasePath = folder };
            Files.Add(newFiles);
            newFiles.ScanFiles(token);
            OnPropertyChanged();
            return Task.CompletedTask;
        }

        public Task AddFolders(List<string> folders, CancellationToken token)
        {
            foreach (var folder in folders)
            {
                _ = AddFolder(folder, token);
            }
            return Task.CompletedTask;
        }

        public void SelectFiles(List<string> selection)
        {
            var files = AllFiles().Where(x=>selection.Contains(x.Path));
            foreach (var file in files)
            {
                file.Selected = true;
            }
        }


        public Task ReloadFolders(CancellationToken token)
        {
            var folders = new List<string>(Folders);
            Application.Current.Dispatcher.Invoke(() => Folders.Clear());
            Files.Clear();
            OnPropertyChanged();
            foreach (var folder in folders)
            {
                _ = AddFolder(folder, token);
            }
            return Task.CompletedTask;
        }

        public void RemoveFolder(int index)
        {
            if (index >= 0)
            {
                Folders.RemoveAt(index);
                Files.RemoveAt(index);
                OnPropertyChanged();
            }
        }

        public IEnumerable<ThirdPartyEntry> AllFiles()
        {
            return Files.SelectMany(x => x.GetAllFiles());
        }

        public IEnumerable<ThirdPartyEntry> AllSelected()
        {
            return AllFiles().Where(x => x.Selected && !x.IsDirectory);
        }

        public (List<ThirdPartyEntry> foundFiles, List<string>remainingFiles) GetFiles(List<string> files)
        {
            var otherPartyFiles = AllFiles();
            var foundFiles = otherPartyFiles.Where(x =>
            {
                var relativePath = Path.GetRelativePath(x.BasePath, x.Path).ToLower().Replace('\\', '/');
                var result = files.Contains(relativePath);
                return result;
            });

            var remainingFiles = new List<string>(files);
            foreach (var file in foundFiles)
            {
                var relativePath = Path.GetRelativePath(file.BasePath, file.Path).ToLower().Replace('\\', '/');
                _ = remainingFiles.Remove(relativePath);
            }

            return (foundFiles.ToList(), remainingFiles);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Application.Current.Dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
        }
        #endregion
    }
}
