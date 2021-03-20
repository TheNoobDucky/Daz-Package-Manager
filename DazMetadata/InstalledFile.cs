using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DazPackage
{
    public interface IContenType 
    {
        public static bool ContentTypeMatches (string sourceContentType) => throw new NotImplementedException();
    }

    public class InstalledFile : IContenType, INotifyPropertyChanged
    {
        public InstalledFile(InstalledPackage package)
        {
            Package = package;
            ProductName = Package.ProductName;
        }

        public InstalledFile() { }

        public string Image { get; set; }
        public string Path { get; set; }
        public string ProductName { get; set; }
        [JsonIgnore] public bool Selected { get { return Package.Selected; } set { Package.Selected = value; OnPropertyChanged(); } }
        private InstalledPackage package = new InstalledPackage();
        public InstalledPackage Package
        {
            get { return package; }
            set
            {
                package = value;
                package.PropertyChanged += Package_PropertyChanged;
            }
        }

        public static bool ContentTypeMatches(string sourceContentType) => false;

        /// <summary>
        /// Pass PropertyChanged event from package back to view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Package_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
