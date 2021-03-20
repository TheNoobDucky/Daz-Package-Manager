using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public class InstalledPose : InstalledFile
    {
        public InstalledPose(InstalledPackage package) : base(package)
        {
        }
        public InstalledPose() { }

        public static new bool ContentTypeMatches(string sourceContentType)
        {
            return sourceContentType == "Preset/Pose";
        }
    }
}//Preset/Pose
