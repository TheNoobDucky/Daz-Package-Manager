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
                {"characters", x=>{ x.CharacterContent |= AssetTypes.Character; } },
                {"expressions", x=>{ x.CharacterContent |= AssetTypes.Expression; } },
                {"poses", x=>{ x.CharacterContent |= AssetTypes.Pose; } },
                {"shapes", x=>{ x.CharacterContent |= AssetTypes.Shape; } },
                {"hair", x=>{ x.CharacterContent |= AssetTypes.Hair; } },
                {"clothing", x=>{ x.CharacterContent |= AssetTypes.Clothing; } },
                {"accessories", x=>{ x.CharacterContent|= AssetTypes.Accessory; } },
                {"anatomy", x=>{ x.CharacterContent|= AssetTypes.Anatomy; } },
                {"props", x=>{ x.CharacterContent|= AssetTypes.Prop; } },
                {"materials", x=>{ x.CharacterContent|= AssetTypes.Material; } },
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
                {"props", x=>{ x.OtherContents |= AssetTypes.Prop; } },
                {"prop", x=>{ x.OtherContents |= AssetTypes.Prop;; } },
                {"animals", x=>{ x.OtherContents |= AssetTypes.Animal; } },
                {"animations", x=>{ x.OtherContents |= AssetTypes.Animation; } },
                {"aniblocks", x=>{ x.OtherContents |= AssetTypes.Animation; } },
                {"environments", x=>{ x.OtherContents |= AssetTypes.Environment; } },
                {"environment", x=>{ x.OtherContents |= AssetTypes.Environment; } },
                {"architecture", x=>{ x.OtherContents |= AssetTypes.Environment; } },
                {"vehicles", x=>{ x.OtherContents |= AssetTypes.Vehicle ; } },
                {"materials", x=>{ x.OtherContents |= AssetTypes.Material ; } },
                {"shader presets", x=>{ x.OtherContents |= AssetTypes.Material ; } },
                {"lights", x=>{ x.OtherContents |= AssetTypes.Light ; } },
                {"light presets", x=>{ x.OtherContents |= AssetTypes.Light ; } },
                {"lighting presets", x=>{ x.OtherContents |= AssetTypes.Light; } },
                {"scripts", x=>{ x.OtherContents |= AssetTypes.Script; } },
                {"scenes", x=>{ x.OtherContents |= AssetTypes.Scene; } },
                {"runtime/textures", x=>{ x.OtherContents |= AssetTypes.Material; } },
                {"runtime/libraries", x=>{ x.OtherContents |= AssetTypes.Poser; } },
                {"camera presets", x=>{ x.OtherContents |= AssetTypes.Camera; } },
                {"render presets", x=>{ x.OtherContents |= AssetTypes.Camera; } },
                {"general", x=>{ x.OtherContents |= AssetTypes.General; } },
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


                if (lookupTable.TryGetValue(name, out var lambda))
                {
                    lambda(packageType);
                }

                switch (name)
                {
                    //case "content/data/daz 3d/":
                    //    Output.Write("Check: " + file.Name, Output.Level.Warning);
                    //    break;

                    default:
                        break;
                }

                if (name.EndsWith("/"))
                {
                    //Output.Write(name, Output.Level.Debug);
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
            //package.CharacterContent |= AssetTypes.Morph;
        }

        private static void FlagGeneration(PackageType package, int generation)
        {
            switch (generation)
            {
                case 5:
                    package.Generation |= Generation.Genesis_1;
                    break;
                case 6:
                    package.Generation |= Generation.Genesis_2;
                    break;
                case 7:
                    package.Generation |= Generation.Genesis_3;
                    break;
                case 8:
                    package.Generation |= Generation.Genesis_8;
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
                    package.Gender |= Gender.Male;
                    break;
                case 'F':
                    package.Gender |= Gender.Female;
                    break;
                case 'B':
                    package.Gender |= Gender.Female;
                    package.Gender |= Gender.Male;
                    break;
                default:
                    break;
            }
        }
    }
}
