using Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DazPackage
{
    /// <summary>
    /// Represent the package manifest file in a daz package.
    /// File format spec see:http://docs.daz3d.com/doku.php/public/software/install_manager/referenceguide/tech_articles/package_manifest/start
    /// </summary>
    public class PackageManifestFile
    {
        public static XElement GetXML(ZipArchiveEntry file)
        {
            try
            {
                return XElement.Load(file.Open());
            }
            catch (XmlException)
            {
                throw new CorruptFileException(file.Name);
            }
        }

        public static List<string> GetFiles(XElement content)
        {
            var fileEntries = content.Elements("File")?.Attributes("VALUE");

            // Trim "content/" from the path since DIM will skip top level folder.
            var Files = content.Elements("File")?.Select(x =>
            {
                var path = x.Attribute("VALUE")?.Value;
                var target = x.Attribute("TARGET")?.Value.Length + 1; // +1 for "/" at the of path
                Debug.Assert(target < path.Length, "Incorrect substring processing in InstallManifestFile");
                return path[target.Value..]; //TODO this part does not work for plugin type.
            }).ToList();
            return Files;
        }

        public static List<string> FindMetadataFile(List<string> files)
        {
            var MetadataFiles = files.Where(x =>
            {
                var y = x.ToLower();
                return y.StartsWith("runtime/support/") && y.EndsWith(".dsx");
            }).ToList();
            return MetadataFiles;
        }
    }
}
