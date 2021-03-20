
namespace DazPackage
{
    /// <summary>
    /// Represents a character that has been installed (ie have an install manifest).
    /// </summary>
    public class InstalledCharacter : InstalledFile
    {
        public InstalledCharacter(InstalledPackage package) : base(package)
        {
        }
        public InstalledCharacter() { }

        public GenerationEnum Generation { get; set; }
        public static new bool ContentTypeMatches(string sourceContentType) 
        {
            return sourceContentType == "Actor/Character" || sourceContentType == "Preset/Character";
        }
    }
}
