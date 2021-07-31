using Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DazPackage
{
    /// <summary>
    /// Represent the install manifest file.
    /// File format spec see:http://docs.daz3d.com/doku.php/public/software/install_manager/referenceguide/tech_articles/install_manifest/start
    /// </summary>
    public class InstallManifestFile
    {
        public InstallManifestFile(FileInfo file)
        {
            try
            {
                using var filestream = file.OpenRead();
                var content = XElement.Load(filestream);
                if (content.Name != "DAZInstallManifest")
                {
                    throw new CorruptFileException("Not install manifest file: " + file.FullName);
                }

                GlobalID = content.Element("GlobalID")?.Attribute("VALUE")?.Value;

                var metadataGUIDElement = content.Element("MetadataGlobalID");

                ProductName = content.Element("ProductName")?.Attribute("VALUE")?.Value;
                ProductStoreID = content.Element("ProductStoreIDX")?.Attribute("VALUE")?.Value;
                UserInstallPath = content.Element("UserInstallPath")?.Attribute("VALUE")?.Value;

                // Only handle content packages not plugins.
                var installedTypes = content.Element("InstallTypes")?.Attribute("VALUE")?.Value;
                if (installedTypes == "Content")
                {
                    Files = PackageManifestFile.GetFiles(content);
                    MetadataFiles = PackageManifestFile.FindMetadataFile(Files);
                }
            }
            catch (XmlException)
            {
                throw new CorruptFileException("File maybe corrupt: " + file.FullName);
            }
        }

        public InstallManifestFile() { }
        public string GlobalID { get; set; }
        public string MetadataGlobalID { get; set; }
        public string ProductName { get; set; }
        public string ProductStoreID { get; set; }
        public string UserInstallPath { get; set; }

        public string ProductID => ProductStoreID.Split('-')[0];
        public string PackageID => ProductStoreID.Split('-').Skip(1).FirstOrDefault();

        public List<string> Files { get; set; } = new List<string>();
        public List<string> MetadataFiles { get; set; } = new List<string>();
    }
}
