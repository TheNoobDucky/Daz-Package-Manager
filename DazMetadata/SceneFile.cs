using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DazPackage
{
    public class SceneFile
    {
        public SceneFile(FileInfo sceneLocation)
        {
            static string GetUrl(JsonElement element)
            {
                var result = new JsonElement();
                return element.TryGetProperty("url", out result) ? result.ToString().ToLower() : "";
            }

            static JsonElement.ArrayEnumerator GetMap(JsonElement element)
            {
                var result = new JsonElement();
                return element.TryGetProperty("map", out result) ? result.EnumerateArray() : new JsonElement.ArrayEnumerator();
            }

            try
            {
                var sceneContent = Helper.ReadJsonFromGZfile(sceneLocation);
                var root = sceneContent.RootElement;

                var imageLibrary = new JsonElement();
                if (root.TryGetProperty("image_library", out imageLibrary))
                {
                    var imageMaps = imageLibrary.EnumerateArray().SelectMany(x => GetMap(x));
                    FilesInSceneLowerCase.UnionWith(imageMaps.Select(GetUrl));
                }

                var scene = new JsonElement();
                if (root.TryGetProperty("scene", out scene))
                {
                    var modifiers = new JsonElement();
                    if (scene.TryGetProperty("modifiers", out modifiers))
                    {
                        FilesInSceneLowerCase.UnionWith(modifiers.EnumerateArray().Select(x => GetUrl(x).Split('#')[0]));
                    }

                    var nodes = new JsonElement();
                    if (scene.TryGetProperty("nodes", out nodes))
                    {
                        // Find the url section of modifier. Get the first part of the string
                        FilesInSceneLowerCase.UnionWith(nodes.EnumerateArray().Select(x => GetUrl(x).Split('#')[0]));
                    }

                    /// TODO materials.
                }

                FilesInSceneLowerCase.Remove("");
                // unscape url and remove leading '/'
                FilesInSceneLowerCase = FilesInSceneLowerCase.Select(x => Uri.UnescapeDataString(x)[1..]).ToHashSet();

            }
            catch (InvalidDataException)
            {
                throw new CorruptFileException(sceneLocation.FullName);
            }

        }
        public HashSet<string> FilesInSceneLowerCase { get; private set; } = new HashSet<string>();

        public static (List<InstalledPackage> packagesInScene, List<string> remainingFiles) PackagesInScene(FileInfo sceneLocation, IEnumerable<InstalledPackage> packages)
        {
            var scene = new SceneFile(sceneLocation);
            var filesInSceneLowerCase = scene.FilesInSceneLowerCase;
            // Look for packages that contain file the scene referenced.
            var packagesInScene = packages.Where(x => x.Files.Any(files => filesInSceneLowerCase.Contains(files.ToLower()))).ToList();

            // Check for files that is not able to find reference to.
            var files = packagesInScene.SelectMany(x => x.Files).Select(x=>x.ToLower()).ToList();
            filesInSceneLowerCase.ExceptWith(files);
            var remainingFiles = filesInSceneLowerCase.ToList();
            return (packagesInScene, remainingFiles);
        }
    }
}
