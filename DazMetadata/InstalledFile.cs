using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace DazPackage
{



    public interface IContenType 
    {
        public static bool ContentTypeMatches (string sourceContentType) => throw new NotImplementedException();
    }

    [Serializable]
    public class InstalledFile : IContenType, INotifyPropertyChanged
    {
        public InstalledFile(InstalledPackage package)
        {
            Package = package;
            Package.PropertyChanged += Package_PropertyChanged;
        }

        public InstalledFile() { }

        public string Image { get; set; }
        public string Path { get; set; }
        public string ProductName { get { return Package.ProductName; } }
        public bool Selected { get { return Package.Selected; } set { Package.Selected = value; OnPropertyChanged(); } }

        public InstalledPackage Package { get; private set; }
        public static bool ContentTypeMatches(string sourceContentType) => false;

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
