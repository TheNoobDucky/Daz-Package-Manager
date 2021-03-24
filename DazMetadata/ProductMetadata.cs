using System.Collections.Generic;
using System.Xml.Linq;

namespace DazPackage
{
    /// <summary>
    /// Represents the product section of a metadata file.
    /// </summary>
    class ProductMetadata
    {
        public ProductMetadata(XElement productXML)
        {
            product = productXML;
        }

        public string ProductName { get { return product.Attribute("VALUE").Value; } }
        public string ProductToken { get { return product.Element("ProductToken").Attribute("VALUE").Value; } }
        public IEnumerable<XElement> Assets { get { return product.Elements("Assets").Elements("Asset"); } }
        private readonly XElement product;
    }
}
