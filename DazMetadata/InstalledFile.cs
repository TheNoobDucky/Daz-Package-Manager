using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public interface IContenType 
    {
        public static bool ContentTypeMatches (string sourceContentType) => throw new NotImplementedException();
    }

    [Serializable]
    public class InstalledFile : IContenType
    {
        public InstalledFile(InstalledPackage package)
        {
            Package = package;
        }
        public InstalledFile() { }

        public string Image { get; set; }
        public string Path { get; set; }

        public string ProductName { get { return Package.ProductName; } }
        public bool Selected { get { return Package.Selected; } set { Package.Selected = value; } }

        public InstalledPackage Package { get; private set; }
        public static bool ContentTypeMatches(string sourceContentType) => false;

    }
}
