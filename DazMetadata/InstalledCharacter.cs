using System.Collections.Generic;

namespace DazPackage
{
    /// <summary>
    /// Represents a character that has been installed (ie have an install manifest).
    /// </summary>
    public class InstalledCharacter : InstalledFile
    {
        public InstalledCharacter(InstalledPackage package, IEnumerable<string> compatibilities) : base(package, compatibilities)
        {
            package.AssetTypes |= AssetTypes.Character;
        }

        public InstalledCharacter() { }

        public static new bool ContentTypeMatches(string sourceContentType) 
        {
            return sourceContentType == "Actor/Character" || sourceContentType == "Preset/Character";
        }
    }
}
