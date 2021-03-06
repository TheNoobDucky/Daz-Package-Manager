using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DazPackage
{
    public class AssetMetadata
    {
        public AssetMetadata(XElement asset)
        {
            Name = asset.Attribute("VALUE")?.Value;
            ContentType = asset.Element("ContentType")?.Attribute("VALUE")?.Value;
            Categories = asset
                .Elements("Categories")
                .Elements("Category")
                .Attributes("VALUE")
                .Select(x => x.Value)
                .ToList();
            Compatibilities = asset
                .Elements("Compatibilities")
                .Elements("Compatibility")
                .Attributes("VALUE")
                .Select(x => x.Value)
                .ToList();
        }
        public AssetMetadata() { }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Compatibilities { get; set; }
    }
}
