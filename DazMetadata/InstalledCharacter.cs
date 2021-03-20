using System;

namespace DazPackage
{
    /// <summary>
    /// Represents a character that has been installed (ie have an install manifest).
    /// </summary>
    [Serializable]
    public class InstalledCharacter : InstalledFile
    {
        public InstalledCharacter(InstalledPackage package) : base(package)
        {
        }
        public InstalledCharacter() { }

        public GenerationEnum Generation { get; set; }
    }
}
