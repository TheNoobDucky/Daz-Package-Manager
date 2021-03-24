using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Text.Json.JsonElement;

namespace DazPackage
{
    public class SceneFile
    {
        public SceneFile(FileInfo sceneLocation)
        {
            static string GetUrl(JsonElement element)
            {
                var result = new JsonElement();
                return element.TryGetProperty("url", out result) ? result.ToString() : "";
            }

            static ArrayEnumerator GetMap(JsonElement element)
            {
                var result = new JsonElement();
                return element.TryGetProperty("map", out result) ? result.EnumerateArray() : new ArrayEnumerator();
            }

            try
            {
                var sceneContent = Helper.ReadJsonFromGZfile(sceneLocation);
                var root = sceneContent.RootElement;

                var imageLibrary = new JsonElement();
                if (root.TryGetProperty("image_library", out imageLibrary))
                {
                    var imageMaps = imageLibrary.EnumerateArray().SelectMany(x => GetMap(x));
                    ReferencedFile.UnionWith(imageMaps.Select(GetUrl));
                }

                var scene = new JsonElement();
                if (root.TryGetProperty("scene", out scene))
                {
                    var modifiers = new JsonElement();
                    if (scene.TryGetProperty("modifiers", out modifiers))
                    {
                        ReferencedFile.UnionWith(modifiers.EnumerateArray().Select(x => GetUrl(x).Split('#')[0]));
                    }

                    var nodes = new JsonElement();
                    if (scene.TryGetProperty("nodes", out nodes))
                    {
                        // Find the url section of modifier. Get the first part of the string
                        ReferencedFile.UnionWith(nodes.EnumerateArray().Select(x => GetUrl(x).Split('#')[0]));
                    }

                    /// TODO materials.
                }

                ReferencedFile.Remove("");
                // unscape url and remove leading '/'
                ReferencedFile = ReferencedFile.Select(x => Uri.UnescapeDataString(x)[1..]).ToHashSet();

            }
            catch (InvalidDataException)
            {
                throw new CorruptFileException(sceneLocation.FullName);
            }

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
