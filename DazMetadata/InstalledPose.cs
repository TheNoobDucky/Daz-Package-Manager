﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public class InstalledPose : InstalledFile
    {
        public InstalledPose(InstalledPackage package, AssetMetadata asset) : base(package, asset)
        {
            package.AssetTypes |= AssetTypes.Pose;
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
