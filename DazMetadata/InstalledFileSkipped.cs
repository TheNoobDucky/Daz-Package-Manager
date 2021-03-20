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
            if (sourceContentType == null || sourceContentType == "")
            {
                return true;
            } 
            else if (sourceContentType.StartsWith("Follower/") || sourceContentType.StartsWith("Script") || sourceContentType== "Preset/Wearables")
            {
                return true;
            }
            else if (sourceContentType == "Preset/Wearables")
            {
                return true; // Clothing?
            }
            else if (sourceContentType.StartsWith("Prop/") || sourceContentType == "Prop")
            {// Prop
                return true;
            } else if (sourceContentType.StartsWith("Preset/Animation"))
            {// Animation
                return true; 
            } else if (sourceContentType.StartsWith("Preset/Morph"))
            {
                return true; // morph
            }

            return sourceContentType switch
            {
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
