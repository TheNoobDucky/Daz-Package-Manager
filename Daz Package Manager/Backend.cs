using DazPackage;
using Output;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Daz_Package_Manager
{
    internal class Backend
    {

        public Backend()
        {
            SelectPackages = new(this);
            ManifestScanner = new(this);
            ThirdPartyScanner = new(this);
            VirtualFolderManager = new(this);
            CacheManager = new(this);
            ViewManager = new(this);
        }

        public PackagesList Packages = new();
        public ThirdPartyFolders ThirdParty { get; private set; } = new();

        public SelectContents SelectPackages { get; private set; }
        public ManifestScanner ManifestScanner { get; private set; }
        public ThirdPartyScanner ThirdPartyScanner { get; private set; }
        public VirtualFolderInstaller VirtualFolderManager { get; private set; }
        public CacheManager CacheManager { get; private set; }
        public ViewManager ViewManager { get; private set; }
        public GUISettings Settings { get; private set; } = new();
    }
}
