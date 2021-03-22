using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Collections.Generic;

using Helpers;

namespace DazPackage
{
    public interface IContenType 
    {
        public static bool ContentTypeMatches (string sourceContentType) => throw new NotImplementedException();
    }

    public class InstalledFile : IContenType, INotifyPropertyChanged
    {
        public InstalledFile(InstalledPackage package, IEnumerable<string> compatibilities)
        {
            Package = package;
            ProductName = Package.ProductName;
            foreach (var compatibility in compatibilities)
            {
                Generation |= GetGeneration(compatibility);
            }
            package.Generation |= Generation;
        }

        public InstalledFile() { }

        public string Image { get; set; }
        public string Path { get; set; }
        public string ProductName { get; set; }
        public Generation Generation { get; set; } = Generation.Unknown;
        public Gender Gender { get; set; } = Gender.Unknown;

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

        protected static Generation GetGeneration(string compatibility)
        {
            return compatibility switch
            {
                string s when s.StartsWith("/Genesis 8/") || s.StartsWith("/Genesis 8.1/")=> Generation.Genesis_8,
                string s when s.StartsWith("/Genesis 3/") => Generation.Genesis_3,
                string s when s.StartsWith("/Genesis 2/") => Generation.Genesis_2,
                "/Genesis" => Generation.Genesis_1,
                string s when s.StartsWith("/Generation4") => Generation.Gen4,
                _ => Generation.Unknown,
            };
        }
    }
}
