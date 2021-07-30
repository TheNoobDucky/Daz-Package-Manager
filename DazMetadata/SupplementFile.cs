using Helpers;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

namespace DazPackage
{
    /// <summary>
    /// Represent the supplement file in a daz package zip.
    /// </summary>
    public class SupplementFile
    {
        public SupplementFile(ZipArchiveEntry file)
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

        public string ProductName
        {
            get
            {
                return xml.Element("ProductName")?.Attribute("VALUE")?.Value;
            }
        }
        private readonly XElement xml;
    }
}
