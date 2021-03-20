using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public class InstalledMaterial : InstalledFile
    {
        public static new bool ContentTypeMatches(string sourceContentType)
        {
            if (sourceContentType != null)
            {
                if (sourceContentType.StartsWith("Preset/Materials") || sourceContentType.StartsWith("Preset/Shader"))
                {
                    return true;
                }
                return sourceContentType switch
                {
                    "Preset/Shader" => true,
                    _ => false,
                };
            }
            return false;
        }
    }
}
