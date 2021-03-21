using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public class InstalledPose : InstalledFile
    {
        public InstalledPose(InstalledPackage package, IEnumerable<string> compatibilities) : base(package, compatibilities)
        {
            package.AssetTypes |= AssetTypes.Character;
        }
        public InstalledPose() { }

        public static new bool ContentTypeMatches(string sourceContentType)
        {
            if (sourceContentType!=null)
            {
                return sourceContentType.StartsWith("Preset/Pose");
            }
            return false;
        }
    }
}
