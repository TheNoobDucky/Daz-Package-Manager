using Helpers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DazPackage
{
    /// <summary>
    /// Used to determine the content of a Daz package zip file based on folder structure.
    /// </summary>
    public class DazPackageFolderStructure
    {
        public delegate void FlagContent(PackageType package);
        private static readonly Dictionary<string, FlagContent> lookupTable;

        static DazPackageFolderStructure()
        {
            var generations = new Dictionary<string, int>()
            {
                {"genesis", 5 },
                {"genesis 2", 6 },
                {"genesis 3", 7 },
                {"genesis 8", 8 },
                {"genesis 8.1", 8 },
            };

            var genders = new Dictionary<string, char>()
            {
                {"", 'B' },
                {"male", 'M' },
                {"female", 'F' },
            };

            var contentTypes = new Dictionary<string, FlagContent>
            {
                {"characters", x=>{ x.CharacterContent.Character = true; } },
                {"expressions", x=>{ x.CharacterContent.Expressions = true; } },
                {"poses", x=>{ x.CharacterContent.Poses = true; } },
                {"shapes", x=>{ x.CharacterContent.Shapes = true; } },
                {"hair", x=>{ x.CharacterContent.Hair = true; } },
                {"clothing", x=>{ x.CharacterContent.Clothing = true; } },
                {"accessories", x=>{ x.CharacterContent.Accessories = true; } },
                {"anatomy", x=>{ x.CharacterContent.Anatomy = true; } },
                {"props", x=>{ x.CharacterContent.Props = true; } },
                {"materials", x=>{ x.CharacterContent.Materials = true; } },
            };

            lookupTable = new Dictionary<string, FlagContent>();

            foreach (var generation in generations)
            {
                var generationNumber = generation.Value;

                foreach (var gender in genders)
                {
                    var spacing = gender.Key == "" ? "" : " ";
                    var characterPath = "content/people/" + generation.Key.ToLower() + spacing + gender.Key + "/";

                    var genderChar = gender.Value;
                    lookupTable.Add(characterPath, x =>
                        {
                            FlagGender(x, genderChar);
                            FlagGeneration(x, generationNumber);
                        }
                    );
                    foreach (var contentType in contentTypes)
                    {
                        var contentPath = characterPath + contentType.Key.ToLower() + "/";
                        lookupTable.Add(contentPath, contentType.Value);
                    }

                    var morphPath = "content/data/daz 3d/" + generation.Key.ToLower() + "/";
                    if (gender.Key == "") // special case for genesis 1
                    {
                        morphPath += "base/morphs/";
                    }
                    else
                    {
                        morphPath += gender.Key + "/morphs/";
                    }

                    lookupTable.Add(morphPath, x =>
                        {
                            FlagIsMorph(x, generation.Value, gender.Value);
                        }
                    );
                }
            }

            lookupTable.Add("content/people/", x =>
            {
                x.IsPeople = true;
            }
            );

            var otherContents = new Dictionary<string, FlagContent>
            {
                {"props", x=>{ x.OtherContents.Props = true; } },
                {"prop", x=>{ x.OtherContents.Props = true; } },
                {"animals", x=>{ x.OtherContents.Animals = true; } },
                {"animations", x=>{ x.OtherContents.Animations = true; } },
                {"aniblocks", x=>{ x.OtherContents.Animations = true; } },
                {"environments", x=>{ x.OtherContents.Environments = true; } },
                {"environment", x=>{ x.OtherContents.Environments = true; } },
                {"architecture", x=>{ x.OtherContents.Environments = true; } },
                {"vehicles", x=>{ x.OtherContents.Vehicles = true; } },
                {"materials", x=>{ x.OtherContents.Materials = true; } },
                {"shader presets", x=>{ x.OtherContents.Materials = true; } },
                {"lights", x=>{ x.OtherContents.Lights = true; } },
                {"light presets", x=>{ x.OtherContents.Lights = true; } },
                {"lighting presets", x=>{ x.OtherContents.Lights = true; } },
                {"scripts", x=>{ x.OtherContents.Scripts = true; } },
                {"scenes", x=>{ x.OtherContents.Scenes = true; } },
                {"runtime/textures", x=>{ x.OtherContents.Materials = true; } },
                {"runtime/libraries", x=>{ x.OtherContents.Posers = true; } },
                {"camera presets", x=>{ x.OtherContents.Cameras = true; } },
                {"render presets", x=>{ x.OtherContents.Cameras = true; } },
                {"general", x=>{ x.OtherContents.General = true; } },
            };

            foreach (var otherContent in otherContents)
            {
                var otherPath = "content/" + otherContent.Key.ToLower() + "/";
                lookupTable.Add(otherPath, otherContent.Value);
            }
        }


        public static PackageType DetermineContentType(FileInfo file)
        {
            using var archive = ZipFile.OpenRead(file.ToString());
            var packageType = new PackageType();

            foreach (var entry in archive.Entries)
            {
                var name = entry.FullName.ToLower();

                FlagContent del = null;

                if (lookupTable.TryGetValue(name, out del))
                {
                    del(packageType);
                }

                switch (name)
                {
                    case "content/data/daz 3d/":
                        Output.Write("Check: " + file.Name, Output.Level.Debug);
                        break;

                    default:
                        break;
                }

                if (name.EndsWith("/"))
                {
                    Output.Write(name, Output.Level.Debug);
                    packageType.MissingDirectory = false;
                }
            }
            return packageType;
        }

        private static void FlagIsMorph(PackageType package, int generation, char gender)
        {
            FlagGender(package, gender);
            FlagGeneration(package, generation);
            package.ImpactLoadSpeed = true;
            package.CharacterContent.Morphs = true;
        }

        private static void FlagGeneration(PackageType package, int generation)
        {
            switch (generation)
            {
                case 5:
                    package.Generation.Genesis1 = true;
                    break;
                case 6:
                    package.Generation.Genesis2 = true;
                    break;
                case 7:
                    package.Generation.Genesis3 = true;
                    break;
                case 8:
                    package.Generation.Genesis8 = true;
                    break;
                default:
                    break;
            }
        }

        private static void FlagGender(PackageType package, char gender)
        {
            switch (gender)
            {
                case 'M':
                    package.Gender.Male = true;
                    break;
                case 'F':
                    package.Gender.Female = true;
                    break;
                case 'B':
                    package.Gender.Both = true;
                    break;
                default:
                    break;
            }
        }
    }
}
