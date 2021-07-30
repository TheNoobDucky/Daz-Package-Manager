using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DazPackage
{
    // Represents duf file.
    public class DufFile
    {
        public static (List<InstalledPackage> packagesInScene, List<string> remainingFiles) PackagesInFile(FileInfo sceneLocation, IEnumerable<InstalledPackage> packages)
        {
            var filesInSceneLowerCase = ExtractReferencedFiles(sceneLocation);

            // Look for packages that contain file the scene referenced.
            var packagesInScene = packages.Where(x => x.Files.Any(files => filesInSceneLowerCase.Contains(files.ToLower()))).ToList();

            // Check for files that is not able to find reference to.
            var files = packagesInScene.SelectMany(x => x.Files).Select(x => x.ToLower()).ToList();
            filesInSceneLowerCase.ExceptWith(files);
            var remainingFiles = filesInSceneLowerCase.ToList();
            return (packagesInScene, remainingFiles);
        }

        public static HashSet<string> ExtractReferencedFiles(FileInfo location)
        {
            try
            {
                try
                {
                    var sceneContent = Helper.ReadJsonFromGZfile(location);
                    return ExtractFilesFromJsonFile(sceneContent);
                }
                catch (InvalidDataException)
                {
                    // Try open file as plain text.
                    var sceneContent = Helper.ReadJsonFromTextFile(location);
                    return ExtractFilesFromJsonFile(sceneContent);
                }
            }
            catch (InvalidDataException e)
            {
                throw new CorruptFileException($"{e.Message} file: {location.FullName}");
            }
            //catch (JsonReaderException e)
            //{
            //    throw new CorruptFileException($"{e.Message} file: {location.FullName}");
            //}
        }

        private static HashSet<string> ExtractFilesFromJsonFile(JsonDocument file)
        {
            var filesInSceneLowerCase = new HashSet<string>();

            var root = file.RootElement;

            var imageLibrary = new JsonElement();
            if (root.TryGetProperty("image_library", out imageLibrary))
            {
                var imageMaps = imageLibrary.EnumerateArray().SelectMany(x => GetMap(x));
                filesInSceneLowerCase.UnionWith(imageMaps.Select(GetUrl));
            }

            var scene = new JsonElement();
            if (root.TryGetProperty("scene", out scene))
            {
                if (scene.TryGetProperty("modifiers", out var modifiers))
                {
                    filesInSceneLowerCase.UnionWith(modifiers.EnumerateArray().Select(x => GetUrl(x).Split('#')[0]));
                }

                if (scene.TryGetProperty("nodes", out var nodes))
                {
                    // Find the url section of modifier. Get the first part of the string
                    filesInSceneLowerCase.UnionWith(nodes.EnumerateArray().Select(x => GetUrl(x).Split('#')[0]));
                }

                if (scene.TryGetProperty("materials", out var materials))
                {
                    // Find the url section of modifier. Get the first part of the string
                    filesInSceneLowerCase.UnionWith(materials.EnumerateArray().Select(x => GetUVSet(x).Split('#')[0]));
                }
            }

            _ = filesInSceneLowerCase.Remove("");
            // unscape url and remove leading '/'
            filesInSceneLowerCase = filesInSceneLowerCase.Select(x => Uri.UnescapeDataString(x)[1..]).ToHashSet();
            return filesInSceneLowerCase;
        }

        private static string GetUrl(JsonElement element)
        {
            return element.TryGetProperty("url", out var result) ? result.ToString().ToLower() : "";
        }

        private static string GetUVSet(JsonElement element)
        {
            return element.TryGetProperty("uv_set", out var result) ? result.ToString().ToLower() : "";
        }

        private static JsonElement.ArrayEnumerator GetMap(JsonElement element)
        {
            return element.TryGetProperty("map", out var result) ? result.EnumerateArray() : new JsonElement.ArrayEnumerator();
        }
    }
}
