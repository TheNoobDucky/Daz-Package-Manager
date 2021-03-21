using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DazPackage
{
    class InstalledFileSkipped : InstalledFile
    {
        public static new bool ContentTypeMatches(string sourceContentType)
        {
            return sourceContentType switch
            {
                string s when s== null || s == "" => true,
                string s when s.StartsWith("Follower/") || s.StartsWith("Script") || s == "Preset/Wearables" => true,
                string s when s == "Preset / Wearables" => true,
                string s when s.StartsWith("Prop/") || s == "Prop" => true,
                string s when s.StartsWith("Preset/Animation") => true,
                string s when s.StartsWith("Preset/Morph") => true,
                "Support" or "Preset/Simulation-Settings" or 
                "Preset/Layered-Image" or
                "Preset/Visibility" or "Preset/Light" or "Preset/Camera" or "Preset/Render-Settings" => true,
                "Preset/Deformer" or "Preset/Properties" or "Preset/Puppeteer" or
                "Set" or "Scene" => true,
                "Actor" => true,
                _ => false,
            };
        }
    }
}
