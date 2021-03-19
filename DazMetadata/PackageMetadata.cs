using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Linq;

namespace DazPackage
{
    /// <summary>
    /// Represent a metadata file located in "\Runtime\Support\"
    /// </summary>
    public class PackageMetadata
    {
        public PackageMetadata(ZipArchiveEntry file) : this(OpenZipFile(file))
        {
        }

        public PackageMetadata (FileInfo file) : this(OpenFile(file))
        {
        }

        private PackageMetadata(XElement xml)
        {
            var Products = xml.Element("Products").Elements("Product");
            Debug.Assert(Products.Count() == 1, "ERROR!!!!!: Assumption only 1 product per metadata dsx file violated.");
            productMetadata = new ProductMetadata(Products.First());
        }

        //public IEnumerable<XElement> Assets { get { return productMetadata.Assets; } }
        public string ProductName { get { return productMetadata.ProductName; } }
        public string ProductToken { get { return productMetadata.ProductToken; } }
        public IEnumerable<XElement> Assets { get { return productMetadata.Assets; } }

        private readonly ProductMetadata productMetadata;

        private static XElement OpenZipFile(ZipArchiveEntry file)
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

        private static XElement OpenFile(FileInfo file)
        {
            using var filestream = file.OpenRead();
            return XElement.Load(filestream);
        }
    }
}
