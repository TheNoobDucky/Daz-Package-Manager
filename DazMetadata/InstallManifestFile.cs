using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Xml;
using Helpers;
using System;
using System.Windows.Media;
using System.Diagnostics;

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
                    var fileEntries = content.Elements("File")?.Attributes("VALUE");
                    
                    // Trim "content/" from the path since DIM will skip top level folder.
                    Files = content.Elements("File")?.Select(x =>
                    { 
                        var path = x.Attribute("VALUE")?.Value;
                        var target = x.Attribute("TARGET")?.Value.Length + 1; // +1 for "/" at the of path
                        Debug.Assert(target < path.Length, "Incorrect substring processing in InstallManifestFile");
                        return path[target.Value..]; //TODO this part does not work for plugin type.
                    }).ToList();

                    MetadataFiles = Files.Where(x =>
                    {
                        var y = x.ToLower();
                        return y.StartsWith("runtime/support/") && y.EndsWith(".dsx");
                    }).ToList();
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

        public List<string> Files { get; set; } = new List<string>();
        public List<string> MetadataFiles { get; set; } = new List<string>();
    }
}
