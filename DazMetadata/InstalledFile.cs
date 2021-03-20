using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    [Serializable]
    public class InstalledFile
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

    }
}
