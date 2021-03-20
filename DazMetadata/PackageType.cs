using System;
using System.Collections.Generic;
using System.Text;

namespace DazPackage
{
    public enum GenerationEnum
    {
        Unknown,V4,Genesis,Genesis2,Genesis3,Genesis8
    }

    public enum GenderEnum
    {
        Unknwon, Female, Male
    }

    public enum AssetTypeEnum
    {
        character, morph, expression, pose, clothing, material, shape
    }

    public class PackageType
    {
        public CharacterContentFlags CharacterContent { get; set; } = new CharacterContentFlags();
        public OtherContentFlags OtherContents { get; set; } = new OtherContentFlags();
        public GenerationFlag Generation { get; set; } = new GenerationFlag();
        public GenderFlag Gender { get; set; } = new GenderFlag();
        public bool ImpactLoadSpeed { get; set; } = false;
        public bool IsPeople { get; set; } = false;
        public bool MissingDirectory { get; set; } = true;
    }

    public class CharacterContentFlags
    {
        public bool Character { get; set; } = false;
        public bool Expressions { get; set; } = false;
        public bool Poses { get; set; } = false;
        public bool Shapes { get; set; } = false;
        public bool Hair { get; set; } = false;
        public bool Clothing { get; set; } = false;
        public bool Accessories { get; set; } = false;
        public bool Anatomy { get; set; } = false;
        public bool Props { get; set; } = false;
        public bool Materials { get; set; } = false;
        public bool Morphs { get; set; } = false;
        public bool Unknown { get; set; } = false;
    }

    public class OtherContentFlags
    {
        public bool Props { get; set; } = false;
        public bool Animals { get; set; } = false;
        public bool Animations { get; set; } = false;
        public bool Environments { get; set; } = false;
        public bool Vehicles { get; set; } = false;
        public bool Materials { get; set; } = false;
        public bool Lights { get; set; } = false;
        public bool Scripts { get; set; } = false;
        public bool Posers { get; set; } = false;
        public bool Scenes { get; set; } = false;
        public bool General { get; set; } = false;
        public bool Cameras { get; set; } = false;
    }

    public class GenerationFlag
    {
        public bool Genesis1 { get; set; } = false;
        public bool Genesis2 { get; set; } = false;
        public bool Genesis3 { get; set; } = false;
        public bool Genesis8 { get; set; } = false;
    }

    public class GenderFlag
    {
        public bool Female { get; set; } = false;
        public bool Male { get; set; } = false;
        public bool Both { get; set; } = false;
    }
}
