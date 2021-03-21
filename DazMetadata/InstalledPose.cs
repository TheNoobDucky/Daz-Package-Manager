using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public class InstalledPose : InstalledFile
    {
        public InstalledPose(InstalledPackage package, IEnumerable<string> compatibilities) : base(package)
        {
            foreach (var compatibility in compatibilities)
            {
                Generation |= GetGeneration(compatibility);
            }
        }
        public InstalledPose() { }

        public Generation Generation { get; set; } = Generation.None;

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
