using System;

namespace DazPackage
{
    /// <summary>
    /// Represents a character that has been installed (ie have an install manifest).
    /// </summary>
    [Serializable]
    public class InstalledCharacter
    {
        public InstalledCharacter(InstalledPackage package)
        {
            Package = package;
        }
        public InstalledCharacter() { }

        public string CharacterName { get; set; }
        
        public string ProductName { get { return Package.ProductName; } }
        public string CharacterImage { get; set; }
        public GenerationEnum Generation { get; set; }
        public InstalledPackage Package { get; private set; }
        public bool Selected { get { return Package.Selected; } set { Package.Selected = value; } }
    }
}
