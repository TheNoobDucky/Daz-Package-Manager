using System.IO.Compression;
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
        public PackageManifestFile(ZipArchiveEntry file)
        {
            try
            {
                xml = XElement.Load(file.Open());
            }
            catch (XmlException)
            {
                throw new CorruptFileException(file.Name);
            }
        }
        private readonly XElement xml;
    }
}
