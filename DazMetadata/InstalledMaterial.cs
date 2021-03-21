using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public class InstalledMaterial : InstalledFile
    {
        public static new bool ContentTypeMatches(string sourceContentType)
        {
            return sourceContentType switch
            {
                string s when s.StartsWith("Preset/Materials") || s.StartsWith("Preset/Fabric") || s.StartsWith("Preset/Shader") => true,
                "Preset/Shader" => true,
                _ => false,
            };
        }
    }
}
