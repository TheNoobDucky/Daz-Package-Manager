using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public class InstalledMaterial : InstalledFile
    {
        public static new bool ContentTypeMatches(string sourceContentType)
        {
            return sourceContentType == "Preset/Materials";
        }
    }
}
