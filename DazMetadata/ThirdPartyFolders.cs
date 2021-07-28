using Helpers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DazPackage
{
    public class ThirdPartyEntry : INotifyPropertyChanged
    {
        public string Location { get; set; }
        public string RelativePath { get; set; }

        private bool selected = false;
        [JsonIgnore]
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        public ThirdPartyFolder ParentFolder { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public class ThirdPartyFolder : INotifyPropertyChanged
    {
        private bool selected = false;
        [JsonIgnore]
        public bool Selected
        {
            get => selected;
            set
            {
                foreach (var folder in Folders)
                {
                    folder.Selected = value;
                    //folder.selected = value;
                }

                foreach (var file in Files)
                {
                    file.Selected = value;
                }
                selected = value;
                OnPropertyChanged();
            }
        }

        public List<ThirdPartyEntry> Files { get; set; } = new();
        public List<ThirdPartyFolder> Folders { get; set; } = new();
        public string Location { get; set; }
        public string FolderName { get; set; }
        public string BasePath { get; set; }

        public Task ScanFiles(CancellationToken token)
        {
            var subFolders = Directory.EnumerateDirectories(Location);
            foreach (var subFolder in subFolders)
            {
                var folderName = Path.GetRelativePath(Location, subFolder);
                var newFolder = new ThirdPartyFolder { Location = subFolder, FolderName = folderName, BasePath = BasePath };
                Folders.Add(newFolder);
                _ = newFolder.ScanFiles(token);
            }

            var files = Directory.EnumerateFiles(Location);
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                var relativePath = Path.GetRelativePath(BasePath, file);
                Files.Add(item: new ThirdPartyEntry { ParentFolder = this, Location = file, RelativePath = relativePath });
            }
            return Task.CompletedTask;
        }

        public IEnumerable<ThirdPartyEntry> GetAllFiles()
        {
            return Folders.SelectMany(x => x.GetAllFiles()).Concat(Files);
        }
        [JsonIgnore]
        public IList Children => new CompositeCollection()
            {
                new CollectionContainer() { Collection = Folders },
                new CollectionContainer() { Collection = Files }
            };

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public class ThirdPartyFolders
    {
        public ObservableCollection<string> Folders { get; set; } = new();
        public ObservableCollection<ThirdPartyFolder> Files { get; set; } = new();

        public Task AddFolder(string folder, CancellationToken token)
        {
            Application.Current.Dispatcher.Invoke(() => Folders.Add(folder));
            var newFiles = new ThirdPartyFolder { Location = folder, FolderName = folder, BasePath = folder };
            _ = newFiles.ScanFiles(token);
            Application.Current.Dispatcher.Invoke(() => Files.Add(newFiles));
            return Task.CompletedTask;
        }

        public void SelectFiles(List<string> selection)
        {
            var files = AllFiles().Where(x => selection.Contains(x.Location));
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
            }
        }

        public IEnumerable<ThirdPartyEntry> AllFiles()
        {
            return Files.SelectMany(x => x.GetAllFiles());
        }

        public IEnumerable<ThirdPartyEntry> AllSelected()
        {
            return AllFiles().Where(x => x.Selected);
        }

        public (List<ThirdPartyEntry> foundFiles, List<string> remainingFiles) GetFiles(List<string> files)
        {
            var otherPartyFiles = AllFiles();
            var foundFiles = otherPartyFiles.Where(file =>
            {
                var relativePath = file.RelativePath.ToLower().Replace('\\', '/');
                var result = files.Contains(relativePath);
                return result;
            });

            var remainingFiles = new List<string>(files);
            foreach (var file in foundFiles)
            {
                var relativePath = file.RelativePath.ToLower().Replace('\\', '/');
                _ = remainingFiles.Remove(relativePath);
            }

            return (foundFiles.ToList(), remainingFiles);
        }
    }
}
