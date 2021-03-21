using System.Collections.Generic;

namespace DazPackage
{
    /// <summary>
    /// Represents a character that has been installed (ie have an install manifest).
    /// </summary>
    public class InstalledCharacter : InstalledFile
    {
        public InstalledCharacter(InstalledPackage package, IEnumerable<string> compatibilities) : base(package)
        {
            foreach (var compatibility in compatibilities)
            {
                Generation |= GetGeneration(compatibility);
            }
            package.AssetTypes |= AssetTypes.Character;
            package.Generation |= Generation;
        }

        public InstalledCharacter() { }

        public Generation Generation { get; set; } = Generation.None;
        public static new bool ContentTypeMatches(string sourceContentType) 
        {
            return sourceContentType == "Actor/Character" || sourceContentType == "Preset/Character";
        }
    }
}
