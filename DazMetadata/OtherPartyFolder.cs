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
    public class OtherPartyEntry : INotifyPropertyChanged
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
        public OtherPartyFolder Folder { get; set; }

        //public OtherPartyEntry(OtherPartyFolder folder, string path, string basePath, bool isDirectory)
        //{
        //    Folder = folder;
        //    Path = path;
        //    BasePath = basePath;
        //    IsDirectory = isDirectory;
        //}

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public class OtherPartyFolder
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

        public List<OtherPartyEntry> Self { get; set; } = new();
        public OtherPartyEntry SelfEntry { get => Self[0]; set => Self[0] = value; }
        public List<OtherPartyEntry> Files { get; set; } = new();
        public List<OtherPartyFolder> Folders { get; set; } = new();

        public string Folder { get; set; }
        public string BasePath { get; set; }

        public Task ScanFiles(CancellationToken token)
        {
            Self.Add(new OtherPartyEntry { Folder = this, Path = Folder, BasePath = BasePath, IsDirectory = true });
            var subFolders = Directory.EnumerateDirectories(Folder);
            foreach (var subFolder in subFolders)
            {
                var newFolder = new OtherPartyFolder { Folder = subFolder, BasePath = BasePath };
                Folders.Add(newFolder);
                newFolder.ScanFiles(token);
            }

            var files = Directory.EnumerateFiles(Folder);
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                Files.Add(item: new OtherPartyEntry { Folder = this, Path = file, BasePath = BasePath, IsDirectory = false });

            }
            return Task.CompletedTask;
        }

        public IEnumerable<OtherPartyEntry> GetAllFiles()
        {
            return Self.Concat(Folders.SelectMany(x => x.GetAllFiles())).Concat(Files);
        }
    }

    public class OtherPartyFolders : INotifyPropertyChanged
    {
        public ObservableCollection<string> Folders { get; set; } = new();
        public List<OtherPartyFolder> Files { get; set; } = new();

        public Task AddFolder(string folder, CancellationToken token)
        {
            Application.Current.Dispatcher.Invoke(() => Folders.Add(folder));
            var newFiles = new OtherPartyFolder { Folder = folder, BasePath = folder };
            Files.Add(newFiles);
            newFiles.ScanFiles(token);
            OnPropertyChanged();
            return Task.CompletedTask;
        }

        public Task AddFolders(List<string> folders, CancellationToken token)
        {
            foreach (var folder in folders)
            {
                AddFolder(folder, token);
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

        public IEnumerable<OtherPartyEntry> AllFiles()
        {
            return Files.SelectMany(x => x.GetAllFiles());
        }

        public IEnumerable<OtherPartyEntry> AllSelected()
        {
            return AllFiles().Where(x => x.Selected && !x.IsDirectory);
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
