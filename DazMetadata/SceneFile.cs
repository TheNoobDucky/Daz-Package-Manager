using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Helpers;
using System.Windows.Media;

namespace DazPackage
{
    public class SceneFile
    {
        public SceneFile(FileInfo sceneLocation)
        {
            var sceneContent = Helper.ReadJsonFromGZfile(sceneLocation);
            var root = sceneContent.RootElement;

            var imageLibrary = root.GetProperty("image_library").EnumerateArray();
            var imageMaps = imageLibrary.SelectMany(x => x.GetProperty("map").EnumerateArray());
            ReferencedFile.UnionWith(imageMaps.Select(x => x.GetProperty("url").ToString()));



            var scene = root.GetProperty("scene");
            var modifiers = scene.GetProperty("modifiers").EnumerateArray();
            // Find the url section of modifier. Get the first part of the string
            ReferencedFile.UnionWith(modifiers.Select(x => x.GetProperty("url").ToString().Split('#')[0]));

            var nodes = scene.GetProperty("nodes").EnumerateArray();
            ReferencedFile.UnionWith(nodes.Select(x => x.GetProperty("url").ToString().Split('#')[0]));

            /// TODO materials.

            // unscape url and remove leading '/'
            ReferencedFile = ReferencedFile.Select(x=> Uri.UnescapeDataString(x)[1..]).ToHashSet();
        }
        public HashSet<string> ReferencedFile { get; private set; } = new HashSet<string>();

        public static IEnumerable<InstalledPackage> PackageInScene(FileInfo sceneLocation, IEnumerable<InstalledPackage> Characters)
        {
            var scene = new SceneFile(sceneLocation);

            // Look for packages that contain file the scene referenced.
            return Characters.Where(x => x.Files.Intersect(scene.ReferencedFile).Any());
        }
    }
}
