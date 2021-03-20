using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Xml;

namespace DazPackage
{
    /// <summary>
    /// Represent the install manifest file.
    /// File format spec see:http://docs.daz3d.com/doku.php/public/software/install_manager/referenceguide/tech_articles/install_manifest/start
    /// </summary>
    [Serializable]
    public class InstallManifestFile
    {
        public InstallManifestFile (FileInfo file)
        {
            try
            {
                using var filestream = file.OpenRead();
                var content = XElement.Load(filestream);

                GlobalID = content.Element("GlobalID").Attribute("VALUE").Value;
                var metadataGUIDElement = content.Element("MetadataGlobalID");
                if (metadataGUIDElement != null)
                {
                    MetadataGlobalID = metadataGUIDElement.Attribute("VALUE").Value;
                }

                ProductName = content.Element("ProductName").Attribute("VALUE").Value;
                ProductStoreID = content.Element("ProductStoreIDX").Attribute("VALUE").Value;
                var userInstallPathElement = content.Element("UserInstallPath");
                if (userInstallPathElement != null)
                {
                    UserInstallPath = userInstallPathElement.Attribute("VALUE").Value;
                }

                var fileEntries = content.Elements("File").Attributes("VALUE");
                Files = content.Elements("File").Select(x =>
                { // Trim "content/" from the path since DIM will skip top level folder.
                var path = x.Attribute("VALUE").Value;
                    var target = x.Attribute("TARGET").Value.Length + 1; // +1 for "/" at the of path
                return path[target..];
                }).ToList();

                MetadataFiles = Files.Where(x =>
                {
                    var y = x.ToLower();
                    return y.StartsWith("runtime/support/") && y.EndsWith(".dsx");
                }).ToList();
            } catch (XmlException)
            {
            }
        }

        public InstallManifestFile() { }
        public string GlobalID { get; set; }
        public string MetadataGlobalID { get; set; }
        public string ProductName  { get; set; }
        public string ProductStoreID { get; set; }
        public string UserInstallPath { get; set; }

        public List<string> Files { get; set; } = new List<string>();
        public List<string> MetadataFiles { get; set; } = new List<string>();
    }
}
